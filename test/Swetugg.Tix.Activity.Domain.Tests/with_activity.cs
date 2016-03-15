using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public abstract class with_activity : TestBase
    {
        protected with_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected override ICommandDispatcher WithDispatcher(IStoreEvents eventStore)
        {
            var host = DomainHost.Build(eventStore);
            return host.Dispatcher;
        }
    }
}