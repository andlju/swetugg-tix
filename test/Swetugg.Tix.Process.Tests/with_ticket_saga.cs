using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Helpers;
using Xunit.Abstractions;

namespace Swetugg.Tix.Process.Tests
{
    public abstract class with_ticket_saga : SagaTestBase
    {
        protected with_ticket_saga(ITestOutputHelper output) : base(output)
        {
        }

        protected override IMessageDispatcher WithDispatcher(Wireup eventStoreWireup,
            ISagaMessageDispatcher sagaMessageDispatcher, IEnumerable<IPipelineHook> extraHooks)
        {
            var host = ProcessHost.Build(eventStoreWireup, sagaMessageDispatcher, new NullLoggerFactory(), extraHooks);
            return host.Dispatcher;
        }
    }
}