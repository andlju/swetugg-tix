using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_ten_seats_from_activity : with_activity
    {
        public when_removing_ten_seats_from_activity(ITestOutputHelper output) : base(output)
        {
        }
        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(10);
        }

        protected override object When()
        {
            return new RemoveSeats()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                Seats = 10
            };
        }

        [Fact]
        public void then_SeatsRemoved_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatsRemoved>());
        }
    }
}