using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_creating_order : with_order
    {
        public when_creating_order(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid ActivityOwnerId = Guid.NewGuid();

        protected override void Setup()
        {

        }

        protected override object When()
        {
            return new CreateOrder() { OrderId = OrderId, ActivityId = ActivityId, ActivityOwnerId = ActivityOwnerId };
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
        public void then_ActivityOwnerId_is_correct()
        {
            var evt = Commits.GetEvent<OrderCreated>();
            Assert.Equal(ActivityOwnerId, evt.ActivityOwnerId);
        }

    }
}