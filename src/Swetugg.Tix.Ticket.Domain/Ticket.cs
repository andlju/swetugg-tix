using System;
using CommonDomain.Core;
using Swetugg.Tix.Ticket.Events;

namespace Swetugg.Tix.Ticket.Domain
{
    public class Ticket : AggregateBase
    {
        private Guid _aggregateId;
        private Guid _ticketTypeId;
        private Guid? _couponId;

        internal static Ticket Build()
        {
            return new Ticket();
        }

        private Ticket()
        {
            
        }

        public Ticket(Guid ticketId, Guid ticketTypeId, Guid? couponId)
        {
            Raise(new TicketCreated()
            {
                AggregateId = ticketId,
                TicketTypeId = ticketTypeId,
                CouponId = couponId
            });
        }

        protected void Raise(EventBase evt)
        {
            if (_aggregateId != Guid.Empty)
                evt.AggregateId = _aggregateId;
            RaiseEvent(evt);
        }

        private void Apply(TicketCreated evt)
        {
            _aggregateId = evt.AggregateId;
            _ticketTypeId = evt.TicketTypeId;
            _couponId = evt.CouponId;
        }
    }
}
