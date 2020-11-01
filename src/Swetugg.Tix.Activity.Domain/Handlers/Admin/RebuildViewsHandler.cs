using NEventStore;
using Swetugg.Tix.Activity.Commands.Admin;
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
        private static Dictionary<string, object> AddRebuildHeaders(Dictionary<string, object> headers, int revision)
        {
            var newHeaders = new Dictionary<string, object>(headers);
            newHeaders.Add("RebuildToRevision", revision);
            return newHeaders;
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
                    Headers = AddRebuildHeaders(e.Headers, stream.StreamRevision),
                    Body = e.Body
                });

                _eventPublisher.Publish(new PublishedEvents { AggregateId = command.ActivityId.ToString(), Events = streamEvents.ToArray() });
            }
        }
    }
}