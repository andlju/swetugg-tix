using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using Swetugg.Tix.Activity.Events;

namespace Swetugg.Tix.Activity.Domain
{
    public class Activity : AggregateBase
    {
        private int _seatsReserved;
        private int _seatLimit;

        private readonly Dictionary<Guid, TicketType> _ticketTypes = new Dictionary<Guid, TicketType>();

        internal static Activity Build()
        {
            return new Activity();
        }

        private Activity()
        {
            
        }

        public Activity(Guid aggregateId) 
        {
            Raise(new ActivityCreated()
            {
                AggregateId = aggregateId
            });
        }

        public void AddSeats(int seats)
        {
            Raise(new SeatsAdded() {Seats = seats});
        }

        public void RemoveSeats(int seats)
        {
            if (_seatLimit - _seatsReserved < seats)
                throw new ActivityException("NotEnoughSeats", "Not enough seats left to remove");
            Raise(new SeatsRemoved() { Seats = seats });
        }

        public void AddTicketType(Guid ticketTypeId)
        {
            Raise(new TicketTypeAdded()
            {
                TicketTypeId = ticketTypeId
            });
        }

        public void RemoveTicketType(Guid ticketTypeId)
        {
            GuardTicketType(ticketTypeId);
            Raise(new TicketTypeRemoved()
            {
                TicketTypeId = ticketTypeId
            });
        }

        public void IncreaseTicketTypeLimit(Guid ticketTypeId, int seats)
        {
            GuardTicketType(ticketTypeId);
            var ticketType = _ticketTypes[ticketTypeId];
            if (ticketType.SeatLimit.GetValueOrDefault(0) + seats > _seatLimit)
            {
                throw new ActivityException("LimitTooHigh", "A ticket type limit cannot be higher than the total number of seats");
            }

            Raise(new TicketTypeLimitIncreased()
            {
                TicketTypeId = ticketTypeId,
                Seats = seats
            });
        }

        public void DecreaseTicketTypeLimit(Guid ticketTypeId, int seats)
        {
            GuardTicketType(ticketTypeId);
            var ticketType = _ticketTypes[ticketTypeId];
            if (ticketType.SeatLimit.GetValueOrDefault(0) - seats < 0)
            {
                throw new ActivityException("LimitTooLow", "The ticket type limit cannot be below 0");
            }
            Raise(new TicketTypeLimitDecreased()
            {
                TicketTypeId = ticketTypeId,
                Seats = seats
            });
        }

        public void RemoveTicketTypeLimit(Guid ticketTypeId)
        {
            throw new NotImplementedException();
        }

        public void ReserveSeat(Guid ticketTypeId, Guid? couponId, string reference)
        {
            if (_seatsReserved >= _seatLimit)
            {
                throw new ActivityException("NoSeatsLeft", "No seats left");
            }
            GuardTicketType(ticketTypeId);

            var ticketType = _ticketTypes[ticketTypeId];
            if (ticketType.SeatLimit.HasValue && ticketType.SeatLimit <= ticketType.SeatsReserved)
            {
                throw new ActivityException("NoSeatsLeft", "No seats left for this ticket type");
            }

            Raise(new SeatReserved()
            {
                TicketTypeId = ticketTypeId,
                CouponId = couponId,
                Reference = reference
            });
        }

        public void ReturnSeat(Guid ticketTypeId, Guid? couponId, string reference)
        {
            GuardTicketType(ticketTypeId);

            Raise(new SeatReturned()
            {
                TicketTypeId = ticketTypeId,
                CouponId = couponId,
                Reference = reference
            });
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void GuardTicketType(Guid ticketTypeId)
        {
            if (!_ticketTypes.ContainsKey(ticketTypeId))
            {
                throw new ActivityException("UnknownTicketType", "No such ticket type");
            }
        }

        protected void Raise(EventBase evt)
        {
            if (evt.AggregateId == Guid.Empty)
                evt.AggregateId = base.Id;
            RaiseEvent(evt);
        }

        private void Apply(ActivityCreated evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(SeatsAdded evt)
        {
            _seatLimit += evt.Seats;            
        }

        private void Apply(SeatsRemoved evt)
        {
            _seatLimit -= evt.Seats;
        }

        private void Apply(TicketTypeAdded evt)
        {
            _ticketTypes.Add(evt.TicketTypeId, new TicketType());
        }

        private void Apply(TicketTypeLimitIncreased evt)
        {
            var seatLimit = _ticketTypes[evt.TicketTypeId].SeatLimit.GetValueOrDefault(0);
            _ticketTypes[evt.TicketTypeId].SeatLimit = seatLimit + evt.Seats;
        }

        private void Apply(TicketTypeLimitDecreased evt)
        {
            var seatLimit = _ticketTypes[evt.TicketTypeId].SeatLimit.GetValueOrDefault(0);
            _ticketTypes[evt.TicketTypeId].SeatLimit = seatLimit - evt.Seats;
        }

        private void Apply(TicketTypeRemoved evt)
        {
            _ticketTypes.Remove(evt.TicketTypeId);
        }

        private void Apply(SeatReserved evt)
        {
            var ticketType = _ticketTypes[evt.TicketTypeId];
            ticketType.SeatsReserved++;

            _seatsReserved++;
        }

        private void Apply(SeatReturned evt)
        {
            var ticketType = _ticketTypes[evt.TicketTypeId];
            ticketType.SeatsReserved--;

            _seatsReserved--;
        }

    }
}
