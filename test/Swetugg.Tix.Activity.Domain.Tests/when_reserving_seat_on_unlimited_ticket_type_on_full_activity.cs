using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_on_unlimited_ticket_type_on_full_activity : with_activity
    {
        public when_reserving_seat_on_unlimited_ticket_type_on_full_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string OrderReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            var activity = Given
                .Activity(ActivityId, UserId);

            activity
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithRemovedTicketTypeLimit(TicketTypeId);
            for (int i = 0; i < 20; i++)
            {
                activity
                    .WithReservedSeat(TicketTypeId, i.ToString());
            }

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
        public void then_SeatReservationFailed_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatReservationFailed>());
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
        public void then_Reason_is_SoldOut()
        {
            Assert.Equal("SoldOut", Commits.GetEvent<SeatReservationFailed>().ReasonCode);
        }
        
    }

}