using System;
using Swetugg.Tix.Activity.Domain.Commands;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_removing_tickettype : with_activity
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