using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public class CreateTicketHandler : IMessageHandler<CreateTicket>
    {
        private readonly IRepository _repository;

        public CreateTicketHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateTicket msg)
        {
            var ticket = new Ticket(msg.TicketId, msg.TicketTypeId, msg.CouponId);
            _repository.Save(ticket, Guid.NewGuid());
        }
    }

    public class ConfirmSeatReservationHandler : TicketMessageHandler<ConfirmSeatReservation>
    {
        public ConfirmSeatReservationHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Ticket ticket, ConfirmSeatReservation cmd)
        {
            ticket.ConfirmSeatReservation();
        }
    }
}