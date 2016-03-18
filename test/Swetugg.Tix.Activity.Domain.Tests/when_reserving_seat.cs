using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat : with_activity
    {
        public when_reserving_seat(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                Reference = "MyRef"
            };
        }

        [Fact]
        public void then_SeatReserved_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<SeatReserved>());
        }
    }
}