using NEventStore;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Ticket.Domain
{
    public class EventPublisherHook : PipelineHookBase
    {
        private readonly IEventPublisher _publisher;

        public EventPublisherHook(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public override void PostCommit(ICommit committed)
        {
            foreach (var evt in committed.Events)
            {
                _publisher.Publish(evt.Body, committed.StreamId);
            }
        }
    }
}