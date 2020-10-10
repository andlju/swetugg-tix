using Microsoft.Extensions.Logging.Abstractions;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Tests.Helpers;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Swetugg.Tix.Ticket.Domain.Tests
{
    public abstract class with_ticket : AggregateTestBase
    {
        protected with_ticket(ITestOutputHelper output) : base(output)
        {
        }

        protected override IMessageDispatcher WithDispatcher(Wireup eventStoreWireup, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var host = DomainHost.Build(eventStoreWireup, new NullEventPublisher(), new NullLoggerFactory(), extraHooks, commandLog);
            return host.Dispatcher;
        }
    }
}
