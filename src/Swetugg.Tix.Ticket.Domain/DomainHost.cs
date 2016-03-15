using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Handlers;

namespace Swetugg.Tix.Ticket.Domain
{
    public class DomainHost
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public static DomainHost Build(IStoreEvents eventStore)
        {
            return new DomainHost(eventStore);
        }

        private DomainHost(IStoreEvents eventStore)
        {
            var repository = new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new CommandDispatcher();

            // Register all command handlers
            dispatcher.Register(() => new CreateTicketHandler(repository));

            _commandDispatcher = dispatcher;
        }

        public ICommandDispatcher Dispatcher => _commandDispatcher;

    }
}