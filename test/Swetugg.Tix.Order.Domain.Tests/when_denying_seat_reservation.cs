using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_denying_seat_reservation : with_order
    {
        public when_denying_seat_reservation(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid ActivityOwnerId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given.Order(OrderId, ActivityId, ActivityOwnerId)
                .WithAddedTicket(TicketTypeId);
        }

        protected override object When()
        {
            return new DenyReservedSeat() { OrderId = OrderId, TicketTypeId = TicketTypeId, ReasonCode = "SoldOut" };
        }

        [Fact]
        public void then_SeatDenied_event_is_raised()
        {
            Assert.True(Commits.HasEvent<SeatDenied>());
        }

        [Fact]
        public void then_ReasonCode_is_correct()
        {
            Assert.Equal("SoldOut", Commits.GetEvent<SeatDenied>().ReasonCode);
        }

        [Fact]
        public void then_TicketId_is_correct()
        {
            Assert.Equal(PreCommits.GetFirstTicketId(TicketTypeId), Commits.GetEvent<SeatDenied>().TicketId);
        }
    }
}