using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_adding_tickettype : with_activity
    {
        public when_adding_tickettype(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId);
        }

        protected Guid TicketTypeId = Guid.NewGuid();

        protected override object When()
        {
            return new AddTicketType()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_TicketTypeAdded_event_is_raised()
        {
            Assert.True(Commits.HasEvent<TicketTypeAdded>());
        }

        [Fact]
        public void then_BucketId_in_published_events_is_correct()
        {
            Assert.Equal(OwnerId.ToString(), Commits.First().BucketId);
        }
    }

    public class when_adding_tickettype_with_duplicate_id : with_activity
    {
        public when_adding_tickettype_with_duplicate_id(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {

            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithTicketType(TicketTypeId);
        }

        protected override object When()
        {
            return new AddTicketType()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }
    }
}