using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_increasing_seat_limit_on_tickettype_with_too_few_seats_left : with_activity
    {
        public when_increasing_seat_limit_on_tickettype_with_too_few_seats_left(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(5)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new IncreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                Seats = 10,
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

        [Fact]
        public void then_ErrorCode_is_LimitTooHigh()
        {
            Assert.Equal("LimitTooHigh", Command.FailureCode);
        }
    }
}