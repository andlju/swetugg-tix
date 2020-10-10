using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain.Core;
using NEventStore.Domain.Persistence.EventStore;

using Swetugg.Tix.Activity.Domain.Handlers;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.Domain
{
    public class DomainHost
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public static DomainHost Build(Wireup eventStoreWireup, IEventPublisher eventPublisher, ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog)
        {
            var hooks = new IPipelineHook[] { new EventPublisherHook(eventPublisher) };
            if (extraHooks != null)
                hooks = hooks.Concat(extraHooks).ToArray();

            var eventStore =
                eventStoreWireup
                    .HookIntoPipelineUsing(hooks)
                    .Build();
            return new DomainHost(eventStore, loggerFactory, commandLog);
        }

        private DomainHost(IStoreEvents eventStore, ILoggerFactory loggerFactory, ICommandLog commandLog)
        {
            var repository = new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());

            // Register all command handlers
            dispatcher.Register(() => new CreateActivityHandler(repository, commandLog));
            dispatcher.Register(() => new AddSeatsHandler(repository, commandLog));
            dispatcher.Register(() => new RemoveSeatsHandler(repository, commandLog));
            dispatcher.Register(() => new AddTicketTypeHandler(repository, commandLog));
            dispatcher.Register(() => new RemoveTicketTypeHandler(repository, commandLog));
            dispatcher.Register(() => new ReserveSeatHandler(repository, commandLog));
            dispatcher.Register(() => new ReturnSeatHandler(repository, commandLog));
            dispatcher.Register(() => new IncreaseTicketTypeLimitHandler(repository, commandLog));
            dispatcher.Register(() => new DecreaseTicketTypeLimitHandler(repository, commandLog));
            dispatcher.Register(() => new RemoveTicketTypeLimitHandler(repository, commandLog));

            _messageDispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher => _messageDispatcher;
    }
}