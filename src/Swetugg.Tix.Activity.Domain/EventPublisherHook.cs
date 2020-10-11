using NEventStore;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.Domain
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
            foreach (var publisher in _publishers)
            {
                var evts = committed.Events.Select(e => new PublishedEvent
                {
                    Body = e.Body,
                    Headers = e.Headers.Union(committed.Headers),
                    EventType = e.Body.GetType().FullName
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