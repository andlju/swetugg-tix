using Swetugg.Tix.Activity.Events;
using System;
using Xunit;

namespace Swetugg.Tix.Activity.ViewBuilder.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var aggregateId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var target = new ActivityOverviewEventApplier();
            var events = new EventBase[]
            {
                new ActivityCreated() { AggregateId = aggregateId, OwnerId = userId },
                new TicketTypeAdded() { AggregateId = aggregateId, TicketTypeId = ticketTypeId }
            };


            var activityOverview = target.ApplyEvents(null, events);

            Assert.Equal(aggregateId, activityOverview.ActivityId);
            Assert.Equal(userId, activityOverview.OwnerId);
            Assert.Single(activityOverview.TicketTypes);
        }
    }
}
