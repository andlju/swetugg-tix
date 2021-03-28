using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using Xunit;

namespace Swetugg.Tix.User.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var activityId = Guid.NewGuid();
            var target = new UserAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new Dictionary<string,string> { ["Activity"] = activityId.ToString()} }
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", "Activity", new { 
                ActivityId = Guid.NewGuid().ToString(),
                OrganizationId = Guid.NewGuid().ToString()
            });
            Assert.False(actual);
        }
    }
}
