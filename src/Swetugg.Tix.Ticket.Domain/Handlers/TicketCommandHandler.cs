using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public abstract class TicketCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : ITicketCommand
    {
        private readonly IRepository _repository;
        private readonly ICommandLog _commandLog;

        protected TicketCommandHandler(IRepository repository, ICommandLog commandLog)
        {
            _repository = repository;
            _commandLog = commandLog;
        }

        public async Task Handle(TCmd cmd)
        {
            await _commandLog.Store(cmd.CommandId, cmd, cmd.TicketId.ToString());
            try
            {
                var ticket = GetTicket(cmd);

                HandleCommand(ticket, cmd);

                _repository.Save(ticket, Guid.NewGuid(), headers =>
                {
                    headers.Add("CommandId", cmd.CommandId.ToString());
                });
                await _commandLog.Complete(cmd.CommandId, ticket.Version);
            }
            catch (TicketException ex)
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

        protected virtual Ticket GetTicket(TCmd cmd)
        {
            var ticket = _repository.GetById<Ticket>(cmd.TicketId);
            return ticket;
        }

        protected abstract void HandleCommand(Ticket ticket, TCmd cmd);
    }
}