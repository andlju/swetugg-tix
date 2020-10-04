using NEventStore;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain
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
            if (committed.Headers.TryGetValue("CommandLog", out var commandLog) && (bool)commandLog == true)
                return;

            foreach (var evt in committed.Events)
            {
                _publisher.Publish(evt.Body);
            }
        }
    }
}