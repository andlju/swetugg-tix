using NEventStore.Domain.Core;
using Swetugg.Tix.Order.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Order.Domain
{
    public class Order : AggregateBase
    {
        class Ticket
        {
            public Guid TicketId { get; set; }
            public Guid TicketTypeId { get; set; }
            public string TicketReference { get; set; }
            public bool Cancelled { get; set; }
        }

        private Guid _ticketTypeId;
        private Guid _activityId;
        private List<Ticket> _tickets = new List<Ticket>();
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
                TicketId = Guid.NewGuid(),
                TicketTypeId = ticketTypeId
            });
        }

        public void ConfirmReservedSeat(Guid ticketTypeId, string ticketReference)
        {
            if (_tickets.Any(t => t.TicketReference == ticketReference))
            {
                throw new OrderException("SeatAlreadyConfirmed", "This seat has already been confirmed.");
            }
            var firstUnconfirmed = _tickets.FirstOrDefault(t => t.TicketTypeId == ticketTypeId && t.TicketReference == null);
            if (firstUnconfirmed == null)
            {
                throw new OrderException("NoSuchTicketType", $"There is no unconfirmed ticket for ticket type {ticketTypeId} on this order");
            }

            Raise(new SeatReserved()
            {
                TicketId = firstUnconfirmed.TicketId,
                TicketReference = ticketReference
            }); ;
        }

        public void ConfirmReturnedSeat(Guid ticketTypeId, string ticketReference)
        {
            var confirmedTicket = _tickets.FirstOrDefault(t => t.TicketReference == t.TicketReference);
            if (confirmedTicket == null)
            {
                throw new OrderException("NoSeatConfirmed", $"No seat has been confirmed with TicketReference {ticketReference}.");
            }
            if (confirmedTicket.TicketTypeId != ticketTypeId)
            {
                throw new OrderException("WrongTicketType", "There was a mismatch in ticket types for the returned seat.");
            }
            Raise(new SeatReturned()
            {
                TicketId = confirmedTicket.TicketId,
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
            _tickets.Add(new Ticket
            {
                TicketId = evt.TicketId,
                TicketTypeId = evt.TicketTypeId
            });
        }

        private void Apply(SeatReserved evt)
        {
            var ticket = _tickets.Find(t => t.TicketId == evt.TicketId);
            ticket.TicketReference = evt.TicketReference;
        }

        private void Apply(SeatReturned evt)
        {
            var ticket = _tickets.Find(t => t.TicketId == evt.TicketId);
            ticket.TicketReference = null;
        }

    }
}
