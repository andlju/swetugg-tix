using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence.EventStore;
using Swetugg.Tix.Activity.Domain.Handlers;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain
{
    public class DomainHost
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public static DomainHost Build(Wireup eventStoreWireup, IEventPublisher eventPubliser, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks)
        {
            var hooks = new IPipelineHook[] {new EventPublisherHook(eventPubliser)};
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
            dispatcher.Register(() => new CreateActivityHandler(repository));
            dispatcher.Register(() => new AddSeatsHandler(repository));
            dispatcher.Register(() => new RemoveSeatsHandler(repository));
            dispatcher.Register(() => new AddTicketTypeHandler(repository));
            dispatcher.Register(() => new RemoveTicketTypeHandler(repository));
            dispatcher.Register(() => new ReserveSeatHandler(repository));
            dispatcher.Register(() => new ReturnSeatHandler(repository));
            dispatcher.Register(() => new IncreaseTicketTypeLimitHandler(repository));
            dispatcher.Register(() => new DecreaseTicketTypeLimitHandler(repository));
            dispatcher.Register(() => new RemoveTicketTypeLimitHandler(repository));

            _messageDispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher => _messageDispatcher;
    }

    class EventPublisherHook : PipelineHookBase
    {
        private readonly IEventPublisher _publisher;

        public EventPublisherHook(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public override void PostCommit(ICommit committed)
        {
            foreach (var evt in committed.Events)
            {
                _publisher.Publish(evt.Body);
            }
        }
    }
}