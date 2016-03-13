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

    }
}