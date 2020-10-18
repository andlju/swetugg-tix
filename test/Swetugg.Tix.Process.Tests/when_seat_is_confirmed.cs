using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Process.Tests
{
    public class when_seat_is_confirmed : with_order_saga
    {
        protected Guid OrderId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        
        protected string TicketReference = Guid.NewGuid().ToString();

        public when_seat_is_confirmed(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given.AddEvent(new Order.Events.OrderCreated()
            {
                AggregateId = OrderId,
                ActivityId = ActivityId,
            });
        }

        protected override object When()
        {
            return new Activity.Events.SeatReserved()
            {
                AggregateId = ActivityId,
                TicketTypeId = TicketTypeId,
                OrderReference = OrderId.ToString(),
                TicketReference = TicketReference
            };
        }

        [Fact]
        public void then_order_seat_is_confirmed()
        {
            Assert.IsType<Order.Commands.ConfirmReservedSeat>(DispatchedMessages.FirstOrDefault());
        }

        [Fact]
        public void then_correct_order_is_confirmed()
        {
            var cmd = (Order.Commands.ConfirmReservedSeat)DispatchedMessages.First();
            Assert.Equal(OrderId, cmd.OrderId);
        }

        [Fact]
        public void then_correct_TicketTypeId_is_used()
        {
            var cmd = (Order.Commands.ConfirmReservedSeat)DispatchedMessages.First();
            Assert.Equal(TicketTypeId, cmd.TicketTypeId);
        }
        [Fact]
        public void then_correct_TicketReference_is_used()
        {
            var cmd = (Order.Commands.ConfirmReservedSeat)DispatchedMessages.First();
            Assert.Equal(TicketReference, cmd.TicketReference);
        }
    }
}