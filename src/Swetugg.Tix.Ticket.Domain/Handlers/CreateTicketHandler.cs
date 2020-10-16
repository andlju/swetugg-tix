using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public class CreateTicketHandler : IMessageHandler<CreateTicket>
    {
        private readonly IRepository _repository;

        public CreateTicketHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task Handle(CreateTicket msg)
        {
            var ticket = new Ticket(msg.TicketId, msg.TicketTypeId, msg.CouponId);
            _repository.Save(ticket, Guid.NewGuid());
            return Task.FromResult(0);
        }
    }
}