using Swetugg.Tix.Order.Commands.Admin;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Domain.Handlers.Admin
{
    public abstract class AdminCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : IOrderAdminCommand
    {
        ICommandLog _commandLog;

        protected AdminCommandHandler(ICommandLog commandLog)
        {
            _commandLog = commandLog;
        }

        protected abstract Task HandleCommand(TCmd command);

        public virtual async Task Handle(TCmd command)
        {
            await _commandLog.Store(command.CommandId, command);

            try
            {
                await HandleCommand(command);
                await _commandLog.Complete(command.CommandId);
            }
            catch (OrderException ex)
            {
                // This is a domain error and shouldn't be retried
                await _commandLog.Fail(command.CommandId, ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                await _commandLog.Fail(command.CommandId, "UnknownError", ex.ToString());
                throw;
            }
        }
    }
}