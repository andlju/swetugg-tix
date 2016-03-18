using System;
using System.Linq;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_tickettype : with_activity
    {

        public when_removing_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new RemoveTicketType()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_TicketTypeRemoved_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<TicketTypeRemoved>());
        }
    }


    public class when_removing_tickettype_with_reserved_tickets : with_activity
    {

        public when_removing_tickettype_with_reserved_tickets(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(5)
                .WithTicketType(TicketTypeId)
                .WithIncreasedTicketTypeLimit(TicketTypeId, 5)
                .WithReservedSeat(TicketTypeId, "Hej");
        }

        protected override object When()
        {
            return new RemoveTicketType()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_no_events_are_raised()
        {
            Assert.Empty(Commits);
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }
    }

}