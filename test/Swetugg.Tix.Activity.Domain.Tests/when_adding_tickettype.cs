using System;
using System.Linq;
using Swetugg.Tix.Activity.Domain.Commands;
using Swetugg.Tix.Activity.Events;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_adding_tickettype : TestBase
    {
        public when_adding_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId);
        }

        protected Guid TicketTypeId = Guid.NewGuid();

        protected override object When()
        {
            return new AddTicketType()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_TicketTypeAdded_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<TicketTypeAdded>());
        }
    }
}