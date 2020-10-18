using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public class ConfirmSeatReservationHandler : TicketCommandHandler<ConfirmSeatReservation>
    {
        public ConfirmSeatReservationHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Ticket ticket, ConfirmSeatReservation cmd)
        {
            ticket.ConfirmSeatReservation();
        }
    }
}