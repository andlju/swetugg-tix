using System;
using System.Linq;
using Swetugg.Tix.Tests.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Commands;
using Swetugg.Tix.Ticket.Events;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Ticket.Domain.Tests
{
    public class when_confirming_seat_reservation : with_ticket
    {
        public when_confirming_seat_reservation(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid TicketId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given.AddCommand(new CreateTicket() { TicketId = TicketId, ActivityId = ActivityId, TicketTypeId = TicketTypeId });
        }

        protected override object When()
        {
            return new ConfirmSeatReservation() { TicketId = TicketId };
        }

        [Fact]
        public void then_SeatReserved_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<SeatReserved>());
        }
    }
}