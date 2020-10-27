using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_confirming_seat_reservation_with_same_reference_again : with_order
    {
        public when_confirming_seat_reservation_with_same_reference_again(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string TicketReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given.Order(OrderId, ActivityId)
                .WithAddedTicket(TicketTypeId)
                .WithReservedSeat(TicketTypeId, TicketReference);
        }

        protected override object When()
        {
            return new ConfirmReservedSeat() { OrderId = OrderId, TicketTypeId = TicketTypeId, TicketReference = TicketReference };
        }

        [Fact]
        public void then_no_new_SeatReserved_event_is_raised()
        {
            Assert.False(Commits.HasEvent<SeatReserved>());
        }

        [Fact]
        public void then_command_fails_with_SeatAlreadyConfirmed_code()
        {
            Assert.Equal("SeatAlreadyConfirmed", Command.FailureCode);
        }

    }
}