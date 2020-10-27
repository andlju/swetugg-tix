using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_confirming_seat_reservation : with_order
    {
        public when_confirming_seat_reservation(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string TicketReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given.Order(OrderId, ActivityId)
                .WithAddedTicket(TicketTypeId);
        }

        protected override object When()
        {
            return new ConfirmReservedSeat() { OrderId = OrderId, TicketTypeId = TicketTypeId, TicketReference = TicketReference };
        }

        [Fact]
        public void then_SeatReserved_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatReserved>());
        }

        [Fact]
        public void then_TicketReference_is_correct()
        {
            Assert.Equal(TicketReference, Commits.GetEvent<SeatReserved>().TicketReference);
        }

        [Fact]
        public void then_TicketId_is_correct()
        {
            Assert.Equal(PreCommits.GetFirstTicketId(TicketTypeId), Commits.GetEvent<SeatReserved>().TicketId);
        }
    }
}