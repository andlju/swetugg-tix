using System;
using Swetugg.Tix.Activity.Domain.Commands;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_tickettype : TestBase
    {
        protected Guid ActivityId = Guid.NewGuid();

        public when_removing_tickettype(ITestOutputHelper output) : base(output)
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
    }
}