using NEventStore.Domain.Core;
using Swetugg.Tix.Ticket.Events;
using System;

namespace Swetugg.Tix.Ticket.Domain
{
    public class Ticket : AggregateBase
    {
        private Guid _ticketTypeId;
        private Guid _activityId;
        private Guid? _couponId;

        private bool _seatReserved;

        internal static Ticket Build()
        {
            return new Ticket();
        }

        private Ticket()
        {

        }

        public Ticket(Guid ticketId, Guid activityId, Guid ticketTypeId, Guid? couponId)
        {
            Raise(new TicketCreated()
            {
                AggregateId = ticketId,
                ActivityId = activityId,
                TicketTypeId = ticketTypeId,
                CouponId = couponId
            });
        }

        public void ConfirmSeatReservation()
        {
            Raise(new SeatReserved());
        }

        protected void Raise(EventBase evt)
        {
            if (evt.AggregateId == Guid.Empty)
                evt.AggregateId = base.Id;
            RaiseEvent(evt);
        }

        private void Apply(TicketCreated evt)
        {
            Id = evt.AggregateId;
            _activityId = evt.ActivityId;
            _ticketTypeId = evt.TicketTypeId;
            _couponId = evt.CouponId;
        }

        private void Apply(SeatReserved evt)
        {
            _seatReserved = true;
        }
    }
}
