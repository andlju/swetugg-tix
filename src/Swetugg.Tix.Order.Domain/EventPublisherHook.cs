using NEventStore;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Order.Domain
{
    public class EventPublisherHook : PipelineHookBase
    {
        private readonly IEnumerable<IEventPublisher> _publishers;

        public EventPublisherHook(IEnumerable<IEventPublisher> publishers)
        {
            _publishers = publishers.ToArray();
        }

        public Dictionary<string, object> CombineHeaders(IDictionary<string, object> first, IDictionary<string, object> second)
        {
            var headers = new Dictionary<string, object>(first);
            foreach (var h in second)
            {
                headers[h.Key] = h.Value;
            }
            return headers;
        }

        public override void PostCommit(ICommit committed)
        {
            var initialRevision = committed.StreamRevision - committed.Events.Count() + 1;
            var evts = committed.Events.Select((e, i) => new PublishedEvent
            {
                AggregateId = committed.StreamId,
                BucketId = committed.BucketId,
                EventType = e.Body.GetType().FullName,
                Revision = initialRevision + i,
                Body = e.Body,
                Headers = CombineHeaders(committed.Headers, e.Headers),
            }).ToArray();

            foreach (var publisher in _publishers)
            {
                publisher.Publish(
                    new PublishedEvents
                    {
                        AggregateId = committed.StreamId,
                        BucketId = committed.BucketId,
                        Events = evts
                    });
            }
        }
    }
}