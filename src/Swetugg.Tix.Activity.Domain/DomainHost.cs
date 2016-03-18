using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using Swetugg.Tix.Activity.Domain.Handlers;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain
{
    public class DomainHost
    {
        private readonly ICommandDispatcher _commandDispatcher;

        
        public static DomainHost Build(Wireup eventStoreWireup)
        {
            return new DomainHost(eventStoreWireup.Build());
        }

        private DomainHost(IStoreEvents eventStore)
        {
            var repository = new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new CommandDispatcher();

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

            _commandDispatcher = dispatcher;
        }

        public ICommandDispatcher Dispatcher => _commandDispatcher;

    }
}