using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_adding_ticket_to_order : with_order
    {
        public when_adding_ticket_to_order(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string Reference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given.Order(OrderId, ActivityId);
        }

        protected override object When()
        {
            return new AddTicket() { OrderId = OrderId, TicketTypeId = TicketTypeId };
        }

        [Fact]
        public void then_TicketAdded_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<TicketAdded>());
        }
        
        [Fact]
        public void then_TicketTypeId_is_correct()
        {
            Assert.Equal(TicketTypeId, Commits.First().GetEvent<TicketAdded>().TicketTypeId);
        }
    }
}