using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
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
                .Activity(ActivityId, UserId, OwnerId)
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
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId,
                OrderReference = OrderReference
            };
        }

        [Fact]
        public void then_TicketTypeId_is_correct()
        {
            Assert.Equal(TicketTypeId, Commits.GetEvent<SeatReservationFailed>().TicketTypeId);
        }

        [Fact]
        public void then_OrderReference_is_correct()
        {
            Assert.Equal(OrderReference, Commits.GetEvent<SeatReservationFailed>().OrderReference);
        }

        [Fact]
        public void then_Reason_is_TicketTypeSoldOut()
        {
            Assert.Equal("TicketTypeSoldOut", Commits.GetEvent<SeatReservationFailed>().ReasonCode);
        }
    }
}