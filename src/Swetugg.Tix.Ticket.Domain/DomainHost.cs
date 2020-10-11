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
        private readonly IMessageDispatcher _messageDispatcher;

        public static DomainHost Build(Wireup eventStoreWireup, IEnumerable<IEventPublisher> eventPublishers, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var hooks = new IPipelineHook[] { new EventPublisherHook(eventPublishers) };
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
            Func<IRepository> repositoryFunc = () => new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());

            // Register all command handlers
            dispatcher.Register(() => new CreateTicketHandler(repositoryFunc()));
            dispatcher.Register(() => new ConfirmSeatReservationHandler(repositoryFunc()));

            _messageDispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher => _messageDispatcher;

    }
}