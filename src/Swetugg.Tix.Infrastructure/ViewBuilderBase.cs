using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public abstract class ViewBuilderBase<TView> : IViewBuilder
        where TView : class, IView
    {
        private readonly IEventApplier<TView> _eventApplier;

        public ViewBuilderBase(IEventApplier<TView> eventApplier)
        {
            _eventApplier = eventApplier;
        }

        public async Task HandleEvents(IEnumerable<PublishedEvent> events)
        {
            foreach (var aggregateEvents in events.GroupBy(e => e.AggregateId))
            {
                var oldView = await GetView(aggregateEvents.Key);

                // Only apply events that are newer than the current revision.
                // Usually this will be all of them
                var unappliedEvents = aggregateEvents.Where(e => oldView == null || e.Revision > oldView.Revision).OrderBy(e => e.Revision).ToArray();
                if (unappliedEvents.Any())
                {
                    var newView = _eventApplier.ApplyEvents(oldView, unappliedEvents.Select(e => e.Body));
                    newView.Revision = unappliedEvents.Last().Revision;
                    await StoreView(oldView, newView);
                }
            }
        }

        protected abstract Task<TView> GetView(string viewId);
        protected abstract Task StoreView(TView oldView, TView newView);
    }
}