using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_increasing_seat_limit_on_tickettype_with_more_reserved_seats : with_activity
    {
        public when_increasing_seat_limit_on_tickettype_with_more_reserved_seats(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithReservedSeat(TicketTypeId, "hej")
                .WithReservedSeat(TicketTypeId, "hej")
                .WithReservedSeat(TicketTypeId, "hej");
        }

        protected override object When()
        {
            return new IncreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId,
                Seats = 2
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
            Assert.Equal("LimitTooLow", Command.FailureCode);
        }
    }
}