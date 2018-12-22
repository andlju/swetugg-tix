using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using Microsoft.Extensions.Logging;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Handlers;

namespace Swetugg.Tix.Ticket.Domain
{
    public class DomainHost
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public static DomainHost Build(Wireup eventStoreWireup, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks)
        {
            IPipelineHook[] hooks = new IPipelineHook[0];
            if (extraHooks != null)
                hooks = hooks.Concat(extraHooks).ToArray();

            var eventStore =
                eventStoreWireup
                    .HookIntoPipelineUsing(hooks)
                    .Build();
            return new DomainHost(eventStore, loggerFactory);
        }

        private DomainHost(IStoreEvents eventStore, ILoggerFactory loggerFactory)
        {
            var repository = new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());

            // Register all command handlers
            dispatcher.Register(() => new CreateTicketHandler(repository));
            dispatcher.Register(() => new ConfirmSeatReservationHandler(repository));

            _messageDispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher => _messageDispatcher;

    }
}