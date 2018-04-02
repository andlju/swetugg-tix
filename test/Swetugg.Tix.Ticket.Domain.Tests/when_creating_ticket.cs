using System;
using System.Linq;
using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Ticket.Commands;
using Swetugg.Tix.Ticket.Events;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Ticket.Domain.Tests
{
    public class when_creating_ticket : with_ticket
    {
        public when_creating_ticket(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid TicketId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            
        }

        protected override object When()
        {
            return new CreateTicket() { TicketId = TicketId, ActivityId = ActivityId, TicketTypeId = TicketTypeId };
        }

        [Fact]
        public void then_TicketCreated_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<TicketCreated>());
        }
    }
}