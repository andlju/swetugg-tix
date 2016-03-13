using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Swetugg.Tix.Activity.Domain.Commands;
using Swetugg.Tix.Activity.Events;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat : TestBase
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

    public class when_reserving_seat_with_no_seats_left : TestBase
    {
        public when_reserving_seat_with_no_seats_left(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithIncreasedTicketTypeLimit(TicketTypeId, 1)
                .WithReservedSeat(TicketTypeId, "ARef");
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
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }

        [Fact]
        public void then_errorcode_is_NotEnoughSeats()
        {
            var activityException = ThrownException as ActivityException;
            Assert.NotNull(activityException);
            Assert.Equal("NoSeatsLeft", activityException.ErrorCode);
        }
    }
}