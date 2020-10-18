using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using ActivityCommands = Swetugg.Tix.Activity.Commands;
using OrderEvents = Swetugg.Tix.Order.Events;


namespace Swetugg.Tix.Process.Tests
{
    public class when_adding_a_ticket_to_an_order : with_order_saga
    {
        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        public when_adding_a_ticket_to_an_order(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given.
                AddEvent(new OrderEvents.OrderCreated()
                {
                    AggregateId = OrderId,
                    ActivityId = ActivityId
                });
        }

        protected override object When()
        {
            return new OrderEvents.TicketAdded()
            {
                AggregateId = OrderId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_try_to_reserve_a_seat()
        {
            Assert.IsType<ActivityCommands.ReserveSeat>(DispatchedMessages.FirstOrDefault());
        }

        [Fact]
        public void then_orderid_is_used_as_reference()
        {
            var cmd = (ActivityCommands.ReserveSeat)DispatchedMessages.First();
            Assert.Equal(OrderId, Guid.Parse(cmd.OrderReference));
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
