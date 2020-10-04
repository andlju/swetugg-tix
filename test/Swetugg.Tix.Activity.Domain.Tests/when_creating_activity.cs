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
            return new CreateActivity() { ActivityId = ActivityId };
        }

        [Fact]
        public void then_ActivityCreated_is_raised()
        {
            Assert.True(Commits.First().HasEvent<ActivityCreated>());
        }

        [Fact]
        public void then_ActivityId_is_correct()
        {
            Assert.Equal(ActivityId.ToString(), Commits.First().StreamId);
        }
    }
}
