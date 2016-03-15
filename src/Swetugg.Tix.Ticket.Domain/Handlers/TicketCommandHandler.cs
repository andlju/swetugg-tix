using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Commands;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public abstract class TicketCommandHandler<TCmd> :
        ICommandHandler<TCmd>
        where TCmd : ITicketCommand
    {
        private readonly IRepository _repository;

        protected TicketCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(TCmd cmd)
        {
            var ticket = GetTicket(cmd.TicketId);

            HandleCommand(ticket, cmd);

            _repository.Save(ticket, Guid.NewGuid());
        }

        protected virtual Ticket GetTicket(Guid ticketId)
        {
            var ticket = _repository.GetById<Ticket>(ticketId);
            return ticket;
        }

        protected abstract void HandleCommand(Ticket ticket, TCmd cmd);
    }
}