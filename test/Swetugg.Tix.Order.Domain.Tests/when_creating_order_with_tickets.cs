using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_creating_order_with_tickets : with_order
    {
        public when_creating_order_with_tickets(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId_1 = Guid.NewGuid();
        protected Guid TicketTypeId_2 = Guid.NewGuid();

        protected override void Setup()
        {

        }

        protected override object When()
        {
            return new CreateOrderWithTickets()
            {
                OrderId = OrderId,
                ActivityId = ActivityId,
                Tickets = new List<CreateOrderWithTickets.TicketOrder> {
                    new CreateOrderWithTickets.TicketOrder { Quantity = 2, TicketTypeId = TicketTypeId_1},
                    new CreateOrderWithTickets.TicketOrder { Quantity = 3, TicketTypeId = TicketTypeId_2}
                }
            };
        }

        [Fact]
        public void then_OrderCreated_event_is_raised()
        {
            Assert.True(Commits.HasEvent<OrderCreated>());
        }

        [Fact]
        public void then_OrderId_is_correct()
        {
            Assert.Equal(OrderId.ToString(), Commits.First().StreamId);
            var evt = Commits.GetEvent<OrderCreated>();
            Assert.Equal(OrderId, evt.AggregateId);
        }

        [Fact]
        public void then_ActivityId_is_correct()
        {
            var evt = Commits.GetEvent<OrderCreated>();
            Assert.Equal(ActivityId, evt.ActivityId);
        }

        [Fact]
        public void then_TicketAddedEvents_are_raised_for_first_tickettype()
        {
            var evts = Commits.GetEvents<TicketAdded>().Where(t => t.TicketTypeId == TicketTypeId_1).ToArray();
            Assert.Equal(2, evts.Length);
        }

        [Fact]
        public void then_TicketAddedEvents_are_raised_for_second_tickettype()
        {
            var evts = Commits.GetEvents<TicketAdded>().Where(t => t.TicketTypeId == TicketTypeId_2).ToArray();
            Assert.Equal(3, evts.Length);
        }
    }

}