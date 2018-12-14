using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using NEventStore;
using NEventStore.Dispatcher;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Events;

namespace Swetugg.Tix.Process
{

    public class SagaFactory : IConstructSagas
    {
        public ISaga Build(Type type, string id)
        {
            if (type == typeof(TicketConfirmationSaga))
            {
                return new TicketConfirmationSaga(id);
            }
            return null;
        }
    }

    public class ProcessHost 
        : IMessageHandler<TicketCreated>
    {
        private readonly ISagaRepository _sagaRepository;

        public ProcessHost(Wireup eventStoreWireup, ISagaMessageDispatcher sagaMessageDispatcher)
        {
            var eventStore = eventStoreWireup
                .HookIntoPipelineUsing(new MessageDispatcherHook(sagaMessageDispatcher))
                .Build();
            _sagaRepository = new SagaEventStoreRepository(eventStore, new SagaFactory());

            var dispatcher = new MessageDispatcher();
            dispatcher.Register<TicketCreated>(() => this);

            Dispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher { get; }

        public void Handle(TicketCreated evt)
        {
            var processId = evt.AggregateId;
            var saga = _sagaRepository.GetById<TicketConfirmationSaga>(processId);
            saga.Transition(evt);

            _sagaRepository.Save(saga, Guid.NewGuid(), null);
        }

        class MessageDispatcherHook : PipelineHookBase
        {
            private readonly ISagaMessageDispatcher _dispatcher;

            public MessageDispatcherHook(ISagaMessageDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public override void PostCommit(ICommit committed)
            {
                var messages = committed.Headers.Where(h => h.Key.StartsWith("UndispatchedMessage."))
                    .OrderBy(m => m.Key);

                foreach (var message in messages)
                {
                    _dispatcher.Dispatch(message);
                }
            }
        }
    }
}