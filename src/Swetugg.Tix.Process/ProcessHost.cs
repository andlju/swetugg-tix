using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Dispatcher;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Events;

namespace Swetugg.Tix.Process
{
    public class ProcessHost 
        : IMessageHandler<TicketCreated>
    {
        private readonly ISagaRepository _sagaRepository;

        public static ProcessHost Build(Wireup eventStoreWireup, ISagaMessageDispatcher sagaMessageDispatcher,
            ILoggerFactory loggerFactory, IEnumerable<IPipelineHook> extraHooks)
        {
            var hooks = new IPipelineHook[] { new MessageDispatcherHook(sagaMessageDispatcher) };
            if (extraHooks != null)
                hooks = hooks.Concat(extraHooks).ToArray();

            var eventStore = eventStoreWireup
                .HookIntoPipelineUsing(hooks)
                .Build();
            return new ProcessHost(eventStore, sagaMessageDispatcher, loggerFactory);
        }

        private ProcessHost(IStoreEvents eventStore, ISagaMessageDispatcher sagaMessageDispatcher, ILoggerFactory loggerFactory)
        {
            _sagaRepository = new SagaEventStoreRepository(eventStore, new SagaFactory());

            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());
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

        class SagaFactory : IConstructSagas
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
    }
}