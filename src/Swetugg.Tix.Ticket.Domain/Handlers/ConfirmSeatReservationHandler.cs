using NEventStore.Domain.Persistence;
using Swetugg.Tix.Ticket.Commands;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
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