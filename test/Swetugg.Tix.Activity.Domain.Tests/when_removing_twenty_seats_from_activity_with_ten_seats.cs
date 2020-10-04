using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_twenty_seats_from_activity_with_ten_seats : with_activity
    {
        public when_removing_twenty_seats_from_activity_with_ten_seats(ITestOutputHelper output) : base(output)
        {
        }
        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(10);
        }

        protected override object When()
        {
            return new RemoveSeats()
            {
                ActivityId = ActivityId,
                Seats = 20
            };
        }

        [Fact]
        public void then_no_event_is_raised()
        {
            Assert.Empty(Commits);
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }
    }
}