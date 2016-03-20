using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Swetugg.Tix.Ticket.Domain.Tests
{
    public abstract class with_ticket : AggregateTestBase
    {
        protected with_ticket(ITestOutputHelper output) : base(output)
        {
        }

        protected override IMessageDispatcher WithDispatcher(Wireup eventStoreWireup)
        {
            var host = DomainHost.Build(eventStoreWireup);
            return host.Dispatcher;
        }
    }
}
