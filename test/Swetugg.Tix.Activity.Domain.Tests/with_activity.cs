using System.Threading.Tasks;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Helpers;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    class NullEventPublisher : IEventPublisher
    {
        public Task Publish(object evt)
        {
            return Task.FromResult(0);
        }
    }
    public abstract class with_activity : AggregateTestBase
    {
        protected with_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected override IMessageDispatcher WithDispatcher(Wireup eventStoreWireup)
        {
            var host = DomainHost.Build(eventStoreWireup, new NullEventPublisher());
            return host.Dispatcher;
        }
    }
}