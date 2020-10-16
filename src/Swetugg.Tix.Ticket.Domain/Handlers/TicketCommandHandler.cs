using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;
using System.Threading.Tasks;

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

        public Task Handle(TCmd msg)
        {
            var ticket = GetTicket(msg.TicketId);

            HandleCommand(ticket, msg);

            _repository.Save(ticket, Guid.NewGuid(), headers =>
            {
                headers.Add("CommandId", msg.CommandId.ToString());
            });
            return Task.FromResult(0);
        }

        protected virtual Ticket GetTicket(Guid ticketId)
        {
            var ticket = _repository.GetById<Ticket>(ticketId);
            return ticket;
        }

        protected abstract void HandleCommand(Ticket ticket, TCmd cmd);
    }
}