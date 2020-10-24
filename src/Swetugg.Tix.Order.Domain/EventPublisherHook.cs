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
            var initialRevision = committed.StreamRevision - committed.Events.Count();
            var revision = initialRevision;
            foreach (var publisher in _publishers)
            {
                revision++;
                var evts = committed.Events.Select(e => new PublishedEvent
                {
                    AggregateId = committed.StreamId,
                    EventType = e.Body.GetType().FullName,
                    Body = e.Body,
                    Revision = revision,
                    Headers = e.Headers.Union(committed.Headers),
                });

                publisher.Publish(
                    new PublishedEvents
                    {
                        AggregateId = committed.StreamId,
                        Events = evts.ToArray()
                    });
            }
        }
    }
}