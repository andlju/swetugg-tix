using System;
using Swetugg.Tix.Activity.Domain.Commands;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public static class ActivityCommandsExtensions
    {
        public static GivenActivityCommands Activity(this IGivenCommands given, Guid activityId)
        {
            given.AddCommand(new CreateActivity()
            {
                ActivityId = activityId
            });
            return new GivenActivityCommands(given, activityId);
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
            given.AddCommand(new ReserveSeat() { TicketTypeId = ticketTypeId, Reference = reference });
            return given;
        }
    }
}