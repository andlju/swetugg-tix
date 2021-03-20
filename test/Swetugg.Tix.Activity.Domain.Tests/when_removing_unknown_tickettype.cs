using Swetugg.Tix.Activity.Commands;
using System;
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
                .Activity(ActivityId, UserId, OwnerId);
        }

        protected override object When()
        {
            return new RemoveTicketType()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = Guid.NewGuid(),
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

    }
}