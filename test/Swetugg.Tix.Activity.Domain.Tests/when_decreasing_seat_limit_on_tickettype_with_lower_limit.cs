using System;
using Swetugg.Tix.Activity.Domain.Commands;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_decreasing_seat_limit_on_tickettype_with_lower_limit : with_activity
    {
        public when_decreasing_seat_limit_on_tickettype_with_lower_limit(ITestOutputHelper output) : base(output)
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
                .WithIncreasedTicketTypeLimit(TicketTypeId, 5);
        }

        protected override object When()
        {
            return new DecreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                Seats = 10
            };
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }

        [Fact]
        public void then_ErrorCode_is_LimitTooHigh()
        {
            var activityException = ThrownException as ActivityException;
            Assert.NotNull(activityException);
            Assert.Equal("LimitTooLow", activityException.ErrorCode);
        }
    }


    public class when_decreasing_seat_limit_on_tickettype_with_more_reserved_seats: with_activity
    {
        public when_decreasing_seat_limit_on_tickettype_with_more_reserved_seats(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(5)
                .WithTicketType(TicketTypeId)
                .WithIncreasedTicketTypeLimit(TicketTypeId, 5)
                .WithReservedSeat(TicketTypeId, "Hej")
                .WithReservedSeat(TicketTypeId, "Hej")
                .WithReservedSeat(TicketTypeId, "Hej");
        }

        protected override object When()
        {
            return new DecreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                Seats = 3
            };
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }

        [Fact]
        public void then_ErrorCode_is_LimitTooLow()
        {
            var activityException = ThrownException as ActivityException;
            Assert.NotNull(activityException);
            Assert.Equal("LimitTooLow", activityException.ErrorCode);
        }
    }
}