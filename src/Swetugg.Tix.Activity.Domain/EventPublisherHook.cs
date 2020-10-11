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
                foreach (var evt in committed.Events)
                {
                    publisher.Publish(evt.Body, committed.StreamId);
                }
            }
        }
    }
}