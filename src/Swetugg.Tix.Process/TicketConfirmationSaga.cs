using System;
using NEventStore.Domain.Core;
using ActivityCommands = Swetugg.Tix.Activity.Commands;
using ActivityEvents = Swetugg.Tix.Activity.Events;
using TicketCommands = Swetugg.Tix.Ticket.Commands;
using TicketEvents = Swetugg.Tix.Ticket.Events;

namespace Swetugg.Tix.Process
{
    public class TicketConfirmationSaga : SagaBase<object>
    {
        private Guid? _activityId;
        private Guid? _ticketTypeId;
        private Guid? _couponId;

        public TicketConfirmationSaga(string id)
        {
            Id = id;
            Register<TicketEvents.TicketCreated>(Handle);
            Register<ActivityEvents.SeatReserved>(Handle);
        }

        void Handle(TicketEvents.TicketCreated evt)
        {
            _activityId = evt.ActivityId;
            _ticketTypeId = evt.TicketTypeId;
            _couponId = evt.CouponId;

            // Try to reserve a seat for this ticket
            Dispatch(new ActivityCommands.ReserveSeat()
            {
                CommandId = Guid.NewGuid(),
                ActivityId = _activityId.Value,
                TicketTypeId = _ticketTypeId.Value,
                CouponId = _couponId,
                Reference = evt.AggregateId.ToString()
            });
        }

        void Handle(ActivityEvents.SeatReserved evt)
        {
            var ticketId = Guid.Parse(evt.Reference);
            // A seat has been reserved for this ticket. Let's confirm it.
            Dispatch(new TicketCommands.ConfirmSeatReservation()
            {
                CommandId = Guid.NewGuid(),
                TicketId = ticketId,
            });
        }
    }
}
