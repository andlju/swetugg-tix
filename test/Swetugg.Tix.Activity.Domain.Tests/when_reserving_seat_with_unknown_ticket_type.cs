using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_with_unknown_ticket_type : with_activity
    {
        public when_reserving_seat_with_unknown_ticket_type(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(20);
        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = Guid.NewGuid(),
                OrderReference = "MyRef"
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

        [Fact]
        public void then_errorcode_is_UnknownTicketType()
        {
            Assert.Equal("UnknownTicketType", Command.FailureCode);
        }
    }
}