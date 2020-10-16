using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process
{
    public class ProcessHost :
        IMessageHandler<Ticket.Events.TicketCreated>,
        IMessageHandler<Activity.Events.SeatReserved>
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
            dispatcher.Register<Ticket.Events.TicketCreated>(() => this);
            dispatcher.Register<Activity.Events.SeatReserved>(() => this);

            Dispatcher = dispatcher;
        }

        public IMessageDispatcher Dispatcher { get; }

        public Task Handle(Ticket.Events.TicketCreated evt)
        {
            var processId = evt.AggregateId;
            var saga = _sagaRepository.GetById<TicketConfirmationSaga>(processId);
            saga.Transition(evt);

            _sagaRepository.Save(saga, Guid.NewGuid(), null);
            return Task.FromResult(0);
        }

        public Task Handle(Activity.Events.SeatReserved evt)
        {
            var processId = Guid.Parse(evt.Reference);
            var saga = _sagaRepository.GetById<TicketConfirmationSaga>(processId);
            saga.Transition(evt);

            _sagaRepository.Save(saga, Guid.NewGuid(), null);
            return Task.FromResult(0);
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
                    _dispatcher.Dispatch(message.Value);
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