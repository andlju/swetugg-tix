using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Helpers;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public abstract class with_activity : AggregateTestBase
    {
        protected with_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected override IMessageDispatcher WithDispatcher(Wireup eventStoreWireup)
        {
            var host = DomainHost.Build(eventStoreWireup);
            return host.Dispatcher;
        }
    }
}