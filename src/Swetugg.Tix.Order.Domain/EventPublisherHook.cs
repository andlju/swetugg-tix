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

        public override void PostCommit(ICommit committed)
        {
            var initialRevision = committed.StreamRevision - committed.Events.Count() + 1;
            var evts = committed.Events.Select((e, i) => new PublishedEvent
            {
                AggregateId = committed.StreamId,
                EventType = e.Body.GetType().FullName,
                Revision = initialRevision + i,
                Body = e.Body,
                Headers = e.Headers.Union(committed.Headers),
            }).ToArray();

            foreach (var publisher in _publishers)
            {
                publisher.Publish(
                    new PublishedEvents
                    {
                        AggregateId = committed.StreamId,
                        Events = evts
                    });
            }
        }
    }
}