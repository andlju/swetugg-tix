using NEventStore;
using Swetugg.Tix.Activity.Commands.Admin;
using Swetugg.Tix.Activity.Events.Admin;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.Domain.Handlers.Admin
{
    public class RebuildViewsHandler : AdminCommandHandler<RebuildViews>
    {
        private IStoreEvents _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public RebuildViewsHandler(IStoreEvents eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public override void Handle(RebuildViews command)
        {
            using (var stream = _eventStore.OpenStream(command.ActivityId))
            {
                var streamEvents = stream.CommittedEvents.Select(e => new PublishedEvent
                {
                    EventType = e.Body.GetType().FullName,
                    Headers = e.Headers,
                    Body = e.Body
                });

                var rebuildViewsEvent = new RebuildViewsRequested { AggregateId = command.ActivityId };

                var events = (new[] { new PublishedEvent { EventType = rebuildViewsEvent.GetType().FullName, Body = rebuildViewsEvent, Headers = null } }).Concat(streamEvents);

                _eventPublisher.Publish(new PublishedEvents { AggregateId = command.ActivityId.ToString(), Events = events.ToArray() });
            }
        }
    }
}