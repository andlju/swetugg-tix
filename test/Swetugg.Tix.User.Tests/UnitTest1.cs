using System;
using Xunit;

namespace Swetugg.Tix.User.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var target = new UserAuthorization();
            var actual = target.HasRoleForObject("Read_Public", "Activity", new { 
                ActivityId = Guid.NewGuid().ToString(),
                OrganizationId = Guid.NewGuid().ToString()
            });
            Assert.False(actual);
        }
    }
}
