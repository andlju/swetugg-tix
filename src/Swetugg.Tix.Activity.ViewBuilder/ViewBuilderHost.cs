using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ViewBuilderHost
    {
        private IDictionary<Type, IList<Func<object, int, Task>>> _eventHandlers = new Dictionary<Type, IList<Func<object, int, Task>>>();

        public void RegisterHandler<TEvent>(IHandleEvent<TEvent> eventHandler)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handlerList))
            {
                _eventHandlers.Add(typeof(TEvent), handlerList = new List<Func<object, int, Task>>());
            };
            handlerList.Add((evt,ver) => eventHandler.Handle((TEvent)evt, ver));
        }

        public static ViewBuilderHost Build(ILoggerFactory loggerFactory)
        {
            return new ViewBuilderHost();
        }

        public async Task HandlePublishedEvent(PublishedEvent evt)
        {
            if (!_eventHandlers.TryGetValue(evt.Body.GetType(), out var handlers))
                return;

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var handler in handlers)
                {
                    await handler(evt.Body, evt.Revision);
                }
                trans.Complete();
            }
        }

    }
}