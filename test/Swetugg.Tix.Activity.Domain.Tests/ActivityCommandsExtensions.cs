using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Tests.Helpers;
using System;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public static class ActivityCommandsExtensions
    {
        public static GivenActivityCommands Activity(this IGivenCommands given, Guid activityId, Guid userId, Guid? createdByUserId = null)
        {
            given.AddCommand(new CreateActivity()
            {
                ActivityId = activityId,
                UserId = createdByUserId ?? userId
            });
            return new GivenActivityCommands(given, activityId, userId);
        }

        public static GivenActivityCommands WithSeats(this GivenActivityCommands given, int seats)
        {
            given.AddCommand(new AddSeats() { Seats = seats });
            return given;
        }

        public static GivenActivityCommands WithTicketType(this GivenActivityCommands given, Guid ticketTypeId)
        {
            given.AddCommand(new AddTicketType() { TicketTypeId = ticketTypeId });
            return given;
        }

        public static GivenActivityCommands WithRemovedTicketTypeLimit(this GivenActivityCommands given, Guid ticketTypeId)
        {
            given.AddCommand(new RemoveTicketTypeLimit() { TicketTypeId = ticketTypeId });
            return given;
        }

        public static GivenActivityCommands WithIncreasedTicketTypeLimit(this GivenActivityCommands given, Guid ticketTypeId, int seats)
        {
            given.AddCommand(new IncreaseTicketTypeLimit() { TicketTypeId = ticketTypeId, Seats = seats });
            return given;
        }

        public static GivenActivityCommands WithDecreasedTicketTypeLimit(this GivenActivityCommands given, Guid ticketTypeId, int seats)
        {
            given.AddCommand(new DecreaseTicketTypeLimit() { TicketTypeId = ticketTypeId, Seats = seats });
            return given;
        }

        public static GivenActivityCommands WithReservedSeat(this GivenActivityCommands given, Guid ticketTypeId, string reference)
        {
            given.AddCommand(new ReserveSeat() { TicketTypeId = ticketTypeId, OrderReference = reference });
            return given;
        }
    }
}