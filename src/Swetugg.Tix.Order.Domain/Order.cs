using NEventStore.Domain.Core;
using Swetugg.Tix.Order.Events;
using System;

namespace Swetugg.Tix.Order.Domain
{
    public class Order : AggregateBase
    {
        private Guid _ticketTypeId;
        private Guid _activityId;

        internal static Order Build()
        {
            return new Order();
        }

        private Order()
        {

        }

        public Order(Guid orderId, Guid activityId)
        {
            Raise(new OrderCreated()
            {
                AggregateId = orderId,
                ActivityId = activityId
            });
        }

        public void AddTicket(Guid ticketTypeId)
        {
            Raise(new TicketAdded()
            {
                TicketTypeId = ticketTypeId
            });
        }

        public void ConfirmReservedSeat(Guid ticketTypeId, string ticketReference)
        {
            Raise(new SeatReserved()
            {
                TicketTypeId = ticketTypeId,
                TicketReference = ticketReference
            });
        }

        public void ConfirmReturnedSeat(Guid ticketTypeId, string ticketReference)
        {
            Raise(new SeatReturned()
            {
                TicketTypeId = ticketTypeId,
                TicketReference = ticketReference
            });
        }

        protected void Raise(EventBase evt)
        {
            if (evt.AggregateId == Guid.Empty)
                evt.AggregateId = base.Id;
            RaiseEvent(evt);
        }

        private void Apply(OrderCreated evt)
        {
            Id = evt.AggregateId;
            _activityId = evt.ActivityId;
        }
        private void Apply(TicketAdded evt)
        {

        }

        private void Apply(SeatReserved evt)
        {
        }

        private void Apply(SeatReturned evt)
        {
        }

    }
}
