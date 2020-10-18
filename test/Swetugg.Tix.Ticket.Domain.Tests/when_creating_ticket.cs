using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Ticket.Commands;
using Swetugg.Tix.Ticket.Events;
using System;
using System.Linq;
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

        [Fact]
        public void then_TicketId_is_correct()
        {
            Assert.Equal(TicketId.ToString(), Commits.First().StreamId);
            var evt = Commits.First().GetEvent<TicketCreated>();
            Assert.Equal(TicketId, evt.AggregateId);
        }

        [Fact]
        public void then_ActivityId_is_correct()
        {
            var evt = Commits.First().GetEvent<TicketCreated>();
            Assert.Equal(ActivityId, evt.ActivityId);
        }

        [Fact]
        public void then_TicketTypeId_is_correct()
        {
            var evt = Commits.First().GetEvent<TicketCreated>();
            Assert.Equal(TicketTypeId, evt.TicketTypeId);
        }
    }
}