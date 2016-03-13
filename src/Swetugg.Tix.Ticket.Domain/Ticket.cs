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

        public Ticket(Guid aggregateId, Guid ticketTypeId, Guid? couponId)
        {
            Raise(new TicketCreated()
            {
                AggregateId = aggregateId,
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

        private void Handle(TicketCreated evt)
        {
            _aggregateId = evt.AggregateId;
            _ticketTypeId = evt.TicketTypeId;
            _couponId = evt.CouponId;
        }
    }
}
