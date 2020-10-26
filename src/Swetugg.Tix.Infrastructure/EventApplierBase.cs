using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Swetugg.Tix.Infrastructure
{
    public interface IEventApplier<TView>
         where TView : class
    {
        TView ApplyEvents(TView view, IEnumerable<object> events);
    }

    public class EventApplierBase<TView> : IEventApplier<TView>
        where TView : class
    {
        private IDictionary<Type, Func<TView, object, TView>> _handlers = new Dictionary<Type, Func<TView, object, TView>>();

        public EventApplierBase()
        {
            RegisterHandlers();
        }

        protected void RegisterHandlers()
        {
            // Look for private Handle-methods
            var allMethods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var handlerMethods = allMethods.Where(m => m.Name == "Handle");
            foreach(var handlerMethod in handlerMethods)
            {
                var pars = handlerMethod.GetParameters();
                if (pars.Length != 2)
                    throw new InvalidOperationException($"Wrong number of parameters on Handle method in {GetType().Name}. Expected 2 but got {pars.Length}.");

                var viewParameter = pars[0];
                if (viewParameter.Name != "view" || viewParameter.ParameterType != typeof(TView))
                    throw new InvalidOperationException($"No view parameter found on {handlerMethod} method. It must be first, be named 'view' and be of type {nameof(TView)}");

                var eventParameter = pars[1];
                if (eventParameter.Name != "evt" || !typeof(object).IsAssignableFrom(eventParameter.ParameterType))
                    throw new InvalidOperationException($"No event parameter found on {handlerMethod} method. It must be second and be named 'evt'");
                
                _handlers.Add(eventParameter.ParameterType, (view, evt) =>
                    (TView)handlerMethod.Invoke(this, new object[] { view, evt })
                );
            }
        }

        public TView ApplyEvents(TView view, IEnumerable<object> events)
        {
            foreach(var evt in events)
            {
                var eventType = evt.GetType();
                if (_handlers.TryGetValue(eventType, out var handler))
                    view = handler(view, evt);
            }
            return view;
        }
    }
}
