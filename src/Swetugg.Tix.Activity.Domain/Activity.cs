using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventStore.Domain.Core;
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

        /// <summary>
        /// Create a new Activity
        /// </summary>
        /// <param name="aggregateId"></param>
        public Activity(Guid aggregateId) 
        {
            Raise(new ActivityCreated()
            {
                AggregateId = aggregateId
            });
        }

        /// <summary>
        /// Add available seats to this activity
        /// </summary>
        /// <param name="seats">Number of seats to add</param>
        public void AddSeats(int seats)
        {
            Raise(new SeatsAdded() {Seats = seats});
        }

        /// <summary>
        /// Remove available seats from this activity
        /// </summary>
        /// <param name="seats">The number of seats to remove</param>
        /// <remarks>If there are not enough free seats left, an <exception cref="ActivityException"></exception> will be thrown.</remarks>
        public void RemoveSeats(int seats)
        {
            if (_seatLimit - _seatsReserved < seats)
                throw new ActivityException("NotEnoughSeats", "Not enough seats left to remove");
            Raise(new SeatsRemoved() { Seats = seats });
        }

        /// <summary>
        /// Add a new ticket type to this activity
        /// </summary>
        /// <param name="ticketTypeId"></param>
        public void AddTicketType(Guid ticketTypeId)
        {
            if (_ticketTypes.ContainsKey(ticketTypeId))
                throw new ActivityException("DuplicateTicketType", "A ticket type with Id {ticketTypeId} already exists");

            Raise(new TicketTypeAdded()
            {
                TicketTypeId = ticketTypeId
            });
        }

        /// <summary>
        /// Remove a ticket type from this activity
        /// </summary>
        /// <param name="ticketTypeId"></param>
        /// <remarks>If the ticket type has reserved seats, an <exception cref="ActivityException"></exception> is thrown</remarks>
        public void RemoveTicketType(Guid ticketTypeId)
        {
            GuardTicketType(ticketTypeId);
            var ticketType = _ticketTypes[ticketTypeId];
            if (ticketType.SeatsReserved > 0)
            {
                throw new ActivityException("TicketTypeInUse", "Unable to remove a ticket type that currently has reserved seats");
            }
            Raise(new TicketTypeRemoved()
            {
                TicketTypeId = ticketTypeId
            });
        }

        /// <summary>
        /// Increase the seat limit of a ticket type
        /// </summary>
        /// <param name="ticketTypeId"></param>
        /// <param name="seats">Number of seats to add to the limit</param>
        /// <remarks>
        /// You can not set a limit higher than the total number of seats for the activity.
        /// You can not set a limit lower than the number of already reserved seats
        /// </remarks>
        public void IncreaseTicketTypeLimit(Guid ticketTypeId, int seats)
        {
            GuardTicketType(ticketTypeId);
            var ticketType = _ticketTypes[ticketTypeId];
            if (!ticketType.SeatLimit.HasValue && ticketType.SeatsReserved > seats)
            {
                throw new ActivityException("LimitTooLow", "You can not set a seat limit that is lower than the number of reserved seats.");
            }
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

        /// <summary>
        /// Decrease the seat limit of a ticket type
        /// </summary>
        /// <param name="ticketTypeId"></param>
        /// <param name="seats">The number of seats to decrease the limit with</param>
        /// <remarks>
        /// You can not set a limit lower than the number of already reserved seats
        /// You can not set a limit lower than 0.
        /// </remarks>
        public void DecreaseTicketTypeLimit(Guid ticketTypeId, int seats)
        {
            GuardTicketType(ticketTypeId);
            var ticketType = _ticketTypes[ticketTypeId];
            if (ticketType.SeatLimit.GetValueOrDefault(0) - seats < ticketType.SeatsReserved)
            {
                throw new ActivityException("LimitTooLow", "You can not set a seat limit that is lower than the number of reserved seats or below zero.");
            }
            Raise(new TicketTypeLimitDecreased()
            {
                TicketTypeId = ticketTypeId,
                Seats = seats
            });
        }

        /// <summary>
        /// Remove the seat limit for this ticket type. 
        /// </summary>
        /// <param name="ticketTypeId"></param>
        public void RemoveTicketTypeLimit(Guid ticketTypeId)
        {
            GuardTicketType(ticketTypeId);

            Raise(new TicketTypeLimitRemoved() { TicketTypeId = ticketTypeId });
        }

        /// <summary>
        /// Reserve a seat for an activity
        /// </summary>
        /// <param name="ticketTypeId">The ticket type this seat is reserved using</param>
        /// <param name="couponId">Optional coupon id</param>
        /// <param name="reference">External reference for this seat (typically the ticket id)</param>
        public void ReserveSeat(Guid ticketTypeId, Guid? couponId, string reference)
        {
            if (_seatsReserved >= _seatLimit)
            {
                throw new ActivityException("NoSeatsLeft", "No seats left");
            }

            var ticketType = GuardTicketType(ticketTypeId);
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

        /// <summary>
        /// Return a seat for this activity
        /// </summary>
        /// <param name="ticketTypeId">The ticket type this seat was reserved using</param>
        /// <param name="couponId">Optional coupon id</param>
        /// <param name="reference">External reference for this seat (typically the ticket id)</param>
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

        private TicketType GuardTicketType(Guid ticketTypeId)
        {
            TicketType ticketType;
            if (!_ticketTypes.TryGetValue(ticketTypeId, out ticketType))
            {
                throw new ActivityException("UnknownTicketType", "No such ticket type");
            }

            return ticketType;
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

        private void Apply(TicketTypeLimitRemoved evt)
        {
            _ticketTypes[evt.TicketTypeId].SeatLimit = null;
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
