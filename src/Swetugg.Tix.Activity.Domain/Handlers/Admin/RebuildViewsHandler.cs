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

        public RebuildViewsHandler(IStoreEvents eventStore, IEventPublisher eventPublisher, ICommandLog commandLog) : base(commandLog)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        protected override void HandleCommand(RebuildViews command)
        {
            using (var stream = _eventStore.OpenStream(command.ActivityId))
            {
                if (stream.StreamRevision == 0)
                {
                    throw new ActivityException("UnknownActivity", $"No Activity found with id {command.ActivityId}");
                }

                var streamEvents = stream.CommittedEvents.Select((e, revision) => new PublishedEvent
                {
                    AggregateId = stream.StreamId,
                    EventType = e.Body.GetType().FullName,
                    Revision = revision + 1,
                    Headers = e.Headers,
                    Body = e.Body
                });

                var rebuildViewsEvent = new RebuildViewsRequested { AggregateId = command.ActivityId };

                var events = (new[] { new PublishedEvent { AggregateId = command.ActivityId.ToString(), EventType = rebuildViewsEvent.GetType().FullName, Body = rebuildViewsEvent, Headers = null } }).Concat(streamEvents);

                _eventPublisher.Publish(new PublishedEvents { AggregateId = command.ActivityId.ToString(), Events = events.ToArray() });
            }
        }
    }
}