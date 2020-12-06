using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Domain.Handlers;
using Swetugg.Tix.Order.Domain.Handlers.Admin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Order.Domain
{
    public class DomainHost
    {
        public static DomainHost Build(Wireup eventStoreWireup, IEventPublisher viewsEventPublisher, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var hooks = new IPipelineHook[] { new EventPublisherHook(new[] { viewsEventPublisher }) };
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
            dispatcher.Register(() => new CreateOrderHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new CreateOrderWithTicketsHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new AddTicketsHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new ConfirmReservedSeatHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new DenyReservedSeatHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new ConfirmReturnedSeatHandler(repositoryFunc(), commandLog));

            // Admin command handlers
            dispatcher.Register(() => new RebuildViewsHandler(eventStore, viewsEventPublisher, commandLog));
            dispatcher.Register(() => new RebuildAllViewsHandler(eventStore, viewsEventPublisher, commandLog));

            Dispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher { get; }

    }
}