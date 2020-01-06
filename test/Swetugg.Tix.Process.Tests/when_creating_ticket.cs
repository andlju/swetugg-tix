using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using ActivityCommands = Swetugg.Tix.Activity.Commands;
using TicketEvents = Swetugg.Tix.Ticket.Events;


namespace Swetugg.Tix.Process.Tests
{
    public class when_creating_ticket : with_ticket_saga
    {
        protected Guid TicketId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        public when_creating_ticket(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {

        }

        protected override object When()
        {
            return new TicketEvents.TicketCreated()
            {
                AggregateId = TicketId,
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                CouponId = null,
            };
        }

        [Fact]
        public void then_try_to_reserve_a_seat()
        {
            Assert.IsType<ActivityCommands.ReserveSeat>(DispatchedMessages.FirstOrDefault());
        }


        [Fact]
        public void then_ticketid_is_used_as_reference()
        {
            var cmd = (ActivityCommands.ReserveSeat)DispatchedMessages.First();
            Assert.Equal(TicketId, Guid.Parse(cmd.Reference));
        }

        [Fact]
        public void then_correct_activity_is_used()
        {
            var cmd = (ActivityCommands.ReserveSeat)DispatchedMessages.First();
            Assert.Equal(ActivityId, cmd.ActivityId);
        }

        [Fact]
        public void then_correct_ticket_type_is_used()
        {
            var cmd = (ActivityCommands.ReserveSeat)DispatchedMessages.First();
            Assert.Equal(TicketTypeId, cmd.TicketTypeId);
        }
    }
}
