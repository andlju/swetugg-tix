using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;

using Swetugg.Tix.Activity.Domain.Handlers;
using Swetugg.Tix.Activity.Domain.Handlers.Admin;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.Domain
{
    public class DomainHost
    {
        public static DomainHost Build(Wireup eventStoreWireup, IEventPublisher domainEventPublisher, IEventPublisher viewsEventPublisher, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var hooks = new IPipelineHook[] { new EventPublisherHook(new[] { domainEventPublisher , viewsEventPublisher }) };
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
            dispatcher.Register(() => new CreateActivityHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new AddSeatsHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new RemoveSeatsHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new AddTicketTypeHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new RemoveTicketTypeHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new ReserveSeatHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new ReturnSeatHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new IncreaseTicketTypeLimitHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new DecreaseTicketTypeLimitHandler(repositoryFunc(), commandLog));
            dispatcher.Register(() => new RemoveTicketTypeLimitHandler(repositoryFunc(), commandLog));

            // Admin command handlers
            dispatcher.Register(() => new RebuildViewsHandler(eventStore, viewsEventPublisher, commandLog));

            Dispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher { get; }
    }
}