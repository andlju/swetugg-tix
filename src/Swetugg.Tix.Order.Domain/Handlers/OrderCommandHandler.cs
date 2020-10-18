using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public abstract class OrderCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : IOrderCommand
    {
        private readonly IRepository _repository;
        private readonly ICommandLog _commandLog;

        protected OrderCommandHandler(IRepository repository, ICommandLog commandLog)
        {
            _repository = repository;
            _commandLog = commandLog;
        }

        public async Task Handle(TCmd cmd)
        {
            await _commandLog.Store(cmd.CommandId, cmd, cmd.OrderId.ToString());
            try
            {
                var order = GetOrder(cmd);

                HandleCommand(order, cmd);

                _repository.Save(order, Guid.NewGuid(), headers =>
                {
                    headers.Add("CommandId", cmd.CommandId.ToString());
                });
                await _commandLog.Complete(cmd.CommandId, order.Version);
            }
            catch (OrderException ex)
            {
                // This is a domain error and shouldn't be retried
                await _commandLog.Fail(cmd.CommandId, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                // This is an infrastructure error or bug. Let's try again a few times.
                await _commandLog.Fail(cmd.CommandId, "UnknownError", ex.ToString());
                throw;
            }
        }

        protected virtual Order GetOrder(TCmd cmd)
        {
            var order = _repository.GetById<Order>(cmd.OrderId);
            return order;
        }

        protected abstract void HandleCommand(Order order, TCmd cmd);
    }
}