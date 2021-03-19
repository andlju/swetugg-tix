using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_creating_activity : with_activity
    {
        public when_creating_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {

        }

        protected override object When()
        {
            return new CreateActivity() { ActivityId = ActivityId, UserId = UserId };
        }

        [Fact]
        public void then_ActivityCreated_is_raised()
        {
            Assert.True(Commits.HasEvent<ActivityCreated>());
        }

        [Fact]
        public void then_ActivityId_is_correct()
        {
            Assert.Equal(ActivityId.ToString(), Commits.First().StreamId);
        }

        [Fact]
        public void then_CreatedByUserId_is_correct()
        {
            var evt = Commits.GetEvent<ActivityCreated>();
            Assert.Equal(UserId, evt.CreatedByUserId);
        }
        
        [Fact]
        public void then_UserId_header_is_correct()
        {
            var actual = Commits.First().Headers["UserId"];
            Assert.Equal(UserId.ToString(), actual as string);
        }
    }
}
