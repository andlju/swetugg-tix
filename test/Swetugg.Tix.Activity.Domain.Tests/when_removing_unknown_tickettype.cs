using System;
using Swetugg.Tix.Activity.Domain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_unknown_tickettype : with_activity
    {
        protected Guid ActivityId = Guid.NewGuid();

        public when_removing_unknown_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given
                .Activity(ActivityId);
        }

        protected override object When()
        {
            return new RemoveTicketType()
            {
                TicketTypeId = Guid.NewGuid()
            };
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsAssignableFrom<ActivityException>(ThrownException);
        }

    }
}