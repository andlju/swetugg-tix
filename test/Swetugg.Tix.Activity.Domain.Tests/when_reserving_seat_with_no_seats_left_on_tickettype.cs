using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_with_no_seats_left_on_tickettype : with_activity
    {
        public when_reserving_seat_with_no_seats_left_on_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();
        protected string OrderReference = Guid.NewGuid().ToString();

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
                OrderReference = OrderReference
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

        [Fact]
        public void then_errorcode_is_correct()
        {
            Assert.Equal("NoSeatsLeft", Command.FailureCode);
        }
    }
}