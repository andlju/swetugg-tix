using System;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_with_no_seats_left : with_activity
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