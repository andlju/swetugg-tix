using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_adding_ten_seats_to_activity : with_activity
    {
        protected Guid ActivityId = Guid.NewGuid();

        public when_adding_ten_seats_to_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given.
                Activity(ActivityId, UserId);
        }

        protected override object When()
        {
            return new AddSeats()
            {
                ActivityId = ActivityId,
                Seats = 10
            };
        }

        [Fact]
        public void then_SeatsAdded_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatsAdded>());
        }
    }
}