using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Process.Tests
{
    public class when_seat_is_confirmed : with_ticket_saga
    {
        protected Guid TicketId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();

        public when_seat_is_confirmed(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given.AddEvent(new Ticket.Events.TicketCreated()
            {
                AggregateId = TicketId,
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId
            });
        }

        protected override object When()
        {
            return new Activity.Events.SeatReserved()
            {
                AggregateId = ActivityId,
                TicketTypeId = TicketTypeId,
                Reference = TicketId.ToString()
            };
        }

        [Fact]
        public void then_ticket_seat_is_confirmed()
        {
            Assert.IsType<Ticket.Commands.ConfirmSeatReservation>(DispatchedMessages.FirstOrDefault());
        }

        [Fact]
        public void then_correct_ticket_is_confirmed()
        {
            var cmd = (Ticket.Commands.ConfirmSeatReservation)DispatchedMessages.First();
            Assert.Equal(TicketId, cmd.TicketId);
        }
    }
}