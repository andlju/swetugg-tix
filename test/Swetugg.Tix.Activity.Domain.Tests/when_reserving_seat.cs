using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{

    public class when_reserving_seat : with_activity
    {
        public when_reserving_seat(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string OrderReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId);
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
        public void then_SeatReserved_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatReserved>());
        }

        [Fact]
        public void then_TicketTypeId_is_correct()
        {
            Assert.Equal(TicketTypeId, Commits.GetEvent<SeatReserved>().TicketTypeId);
        }

        [Fact]
        public void then_OrderReference_is_correct()
        {
            Assert.Equal(OrderReference, Commits.GetEvent<SeatReserved>().OrderReference);
        }

        [Fact]
        public void then_a_new_ticket_reference_is_set()
        {
            Assert.NotNull(Commits.GetEvent<SeatReserved>().TicketReference);
        }
    }
}