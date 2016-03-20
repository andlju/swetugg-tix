using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Handlers;

namespace Swetugg.Tix.Ticket.Domain
{
    public class DomainHost
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public static DomainHost Build(Wireup eventStoreWireup)
        {
            return new DomainHost(eventStoreWireup.Build());
        }

        private DomainHost(IStoreEvents eventStore)
        {
            var repository = new EventStoreRepository(eventStore, new AggregateFactory(), new ConflictDetector());
            var dispatcher = new MessageDispatcher();

            // Register all command handlers
            dispatcher.Register(() => new CreateTicketHandler(repository));
            dispatcher.Register(() => new ConfirmSeatReservationHandler(repository));

            _messageDispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher => _messageDispatcher;

    }
}