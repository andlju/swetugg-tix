using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_increasing_seat_limit_on_tickettype : with_activity
    {
        public when_increasing_seat_limit_on_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new IncreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId,
                Seats = 10,
            };
        }

        [Fact]
        public void then_TicketTypeLimitIncreased_event_is_raised()
        {
            Assert.True(Commits.HasEvent<TicketTypeLimitIncreased>());
        }
    }
}