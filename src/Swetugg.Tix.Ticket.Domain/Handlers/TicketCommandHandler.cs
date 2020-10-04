using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public abstract class TicketMessageHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : ITicketCommand
    {
        private readonly IRepository _repository;

        protected TicketMessageHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(TCmd msg)
        {
            var ticket = GetTicket(msg.TicketId);

            HandleCommand(ticket, msg);

            _repository.Save(ticket, Guid.NewGuid(), headers =>
            {
                headers.Add("CommandId", msg.CommandId.ToString());
            });
        }

        protected virtual Ticket GetTicket(Guid ticketId)
        {
            var ticket = _repository.GetById<Ticket>(ticketId);
            return ticket;
        }

        protected abstract void HandleCommand(Ticket ticket, TCmd cmd);
    }
}