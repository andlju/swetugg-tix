using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Domain;
using NEventStore.Domain.Persistence;
using NEventStore.Domain.Persistence.EventStore;
using Polly;
using Polly.Registry;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process
{
    public class ProcessHost :
        IMessageHandler<Order.Events.OrderCreated>,
        IMessageHandler<Order.Events.TicketAdded>,
        IMessageHandler<Order.Events.TicketCancelled>,
        IMessageHandler<Activity.Events.SeatReserved>,
        IMessageHandler<Activity.Events.SeatReservationFailed>,
        IMessageHandler<Activity.Events.SeatReturned>
    {
        private readonly ISagaRepository _sagaRepository;
        private readonly IPolicyRegistry<string> _policyRegistry;
        private readonly IMessageDispatcher _dispatcher;

        public IMessageDispatcher Dispatcher => _dispatcher;

        public static ProcessHost Build(
            Wireup eventStoreWireup,
            ISagaMessageDispatcher sagaMessageDispatcher,
            ILoggerFactory loggerFactory,
            IEnumerable<IPipelineHook> extraHooks,
            IPolicyRegistry<string> policyRegistry)
        {
            var hooks = new IPipelineHook[] { new MessageDispatcherHook(sagaMessageDispatcher) };
            if (extraHooks != null)
                hooks = hooks.Concat(extraHooks).ToArray();

            var eventStore = eventStoreWireup
                .HookIntoPipelineUsing(hooks)
                .Build();
            return new ProcessHost(eventStore, loggerFactory, policyRegistry);
        }

        private ProcessHost(IStoreEvents eventStore, ILoggerFactory loggerFactory, IPolicyRegistry<string> policyRegistry)
        {
            _sagaRepository = new SagaEventStoreRepository(eventStore, new SagaFactory());
            _policyRegistry = policyRegistry;

            var dispatcher = new MessageDispatcher(loggerFactory.CreateLogger<MessageDispatcher>());
            
            dispatcher.Register<Order.Events.OrderCreated>(() => this);
            dispatcher.Register<Order.Events.TicketAdded>(() => this);
            dispatcher.Register<Order.Events.TicketCancelled>(() => this);

            dispatcher.Register<Activity.Events.SeatReserved>(() => this);
            dispatcher.Register<Activity.Events.SeatReservationFailed>(() => this);
            dispatcher.Register<Activity.Events.SeatReturned>(() => this);

            _dispatcher = dispatcher;
        }

        public Task Dispatch(object msg)
        {
            return _policyRegistry.Get<IAsyncPolicy>("ProcessHost").ExecuteAsync(() => Dispatcher.Dispatch(msg, false));
        }

        protected Task HandleOrderEvent(Order.Events.EventBase evt)
        {
            var processId = evt.AggregateId;
            var saga = _sagaRepository.GetById<OrderConfirmationSaga>(processId);
            saga.Transition(evt);

            _sagaRepository.Save(saga, Guid.NewGuid(), null);
            return Task.FromResult(0);
        }

        public Task Handle(Order.Events.OrderCreated evt)
        {
            return HandleOrderEvent(evt);
        }

        public Task Handle(Order.Events.TicketAdded evt)
        {
            return HandleOrderEvent(evt);
        }

        public Task Handle(Order.Events.TicketCancelled evt)
        {
            return HandleOrderEvent(evt);
        }

        public Task HandleActivityEvent(Activity.Events.EventBase evt, string orderReference)
        {
            var processId = Guid.Parse(orderReference);
            var saga = _sagaRepository.GetById<OrderConfirmationSaga>(processId);
            saga.Transition(evt);

            _sagaRepository.Save(saga, Guid.NewGuid(), null);
            return Task.FromResult(0);

        }

        public Task Handle(Activity.Events.SeatReserved evt)
        {
            return HandleActivityEvent(evt, evt.OrderReference);
        }

        public Task Handle(Activity.Events.SeatReservationFailed evt)
        {
            return HandleActivityEvent(evt, evt.OrderReference);
        }

        public Task Handle(Activity.Events.SeatReturned evt)
        {
            return HandleActivityEvent(evt, evt.OrderReference);
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
                    _dispatcher.Dispatch(message.Value).Wait();
                }
            }
        }

        class SagaFactory : IConstructSagas
        {
            public ISaga Build(Type type, string id)
            {
                if (type == typeof(OrderConfirmationSaga))
                {
                    return new OrderConfirmationSaga(id);
                }
                return null;
            }
        }
    }
}