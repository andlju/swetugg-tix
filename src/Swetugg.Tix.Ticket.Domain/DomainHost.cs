using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Ticket.Domain
{
    public class DomainHost
    {
        public static DomainHost Build(Wireup eventStoreWireup, IEventPublisher domainEventPublisher, IEventPublisher viewsEventPublisher, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var hooks = new IPipelineHook[] { new EventPublisherHook(new[] { domainEventPublisher, viewsEventPublisher }) };
            if (extraHooks != null)
                hooks = hooks.Concat(extraHooks).ToArray();

            var eventStore =
                eventStoreWireup
                    .HookIntoPipelineUsing(hooks)
                    .Build();
            return new DomainHost(eventStore, loggerFactory, commandLog, viewsEventPublisher);
        }

        private DomainHost(IStoreEvents eventStore, ILoggerFactory loggerFactory, ICommandLog commandLog, IEventPublisher viewsEventPublisher)
        {
            Func<IRepository> repositoryFunc = () => new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());

            // Register all command handlers
            dispatcher.Register(() => new CreateTicketHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new ConfirmSeatReservationHandler(repositoryFunc(), commandLog));

            // Admin command handlers
            // dispatcher.Register(() => new RebuildViewsHandler(eventStore, viewsEventPublisher, commandLog));

            Dispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher { get; }

    }
}