using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_on_unknown_tickettype : with_activity
    {
        public when_reserving_seat_on_unknown_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();
        protected Guid UnknownTicketTypeId = Guid.NewGuid();

        protected string OrderReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = UnknownTicketTypeId,
                OrderReference = OrderReference
            };
        }

        [Fact]
        public void then_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

        [Fact]
        public void then_error_message_is_correct()
        {
            Assert.Equal("UnknownTicketType", Command.FailureCode);
        }
    }
}