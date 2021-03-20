using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Infrastructure
{
    public abstract class ViewBuilderBase<TView> : IViewBuilder
        where TView : class, IView
    {
        private readonly IEventApplier<TView> _eventApplier;
        private readonly bool _randomErrors;
        private readonly Random _rnd = new Random();

        public ViewBuilderBase(IEventApplier<TView> eventApplier, bool randomErrors = false)
        {
            _eventApplier = eventApplier;
            _randomErrors = randomErrors;
        }
        private void ThrowRandomError()
        {
            if (_randomErrors && _rnd.Next(100) > 75)
                throw new Exception("Random error");
        }

        public async Task HandleEvents(IEnumerable<PublishedEvent> events)
        {
            foreach (var aggregateEvents in events.GroupBy(e => new { e.BucketId, e.AggregateId }))
            {
                var oldView = await GetView(aggregateEvents.Key.BucketId, aggregateEvents.Key.AggregateId);
                ThrowRandomError();

                // Only apply events that are newer than the current revision.
                // Usually this will be all of them
                var unappliedEvents = aggregateEvents.Where(e => oldView == null || e.Revision > oldView.Revision || IsRebuild(e)).OrderBy(e => e.Revision).ToArray();
                if (unappliedEvents.Any())
                {
                    var newView = _eventApplier.ApplyEvents(oldView, unappliedEvents.Select(e => e.Body));
                    newView.Revision = unappliedEvents.Last().Revision;
                    await StoreView(oldView, newView);
                    ThrowRandomError();
                }
            }
        }

        private bool IsRebuild(PublishedEvent e)
        {
            return e.Headers != null && e.Headers.ContainsKey("RebuildToRevision");
        }

        protected abstract Task<TView> GetView(string bucketId, string viewId);
        protected abstract Task StoreView(TView oldView, TView newView);
    }
}