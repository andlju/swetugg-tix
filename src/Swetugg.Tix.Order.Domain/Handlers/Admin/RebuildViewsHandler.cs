using NEventStore;
using Swetugg.Tix.Order.Commands.Admin;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Swetugg.Tix.Order.Events;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Domain.Handlers.Admin
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

        protected async override Task HandleCommand(RebuildViews command)
        {
            using (var stream = _eventStore.OpenStream(command.OrderId))
            {
                if (stream.StreamRevision == 0)
                {
                    throw new OrderException("UnknownOrder", $"No Order found with id {command.OrderId}");
                }

                var streamEvents = stream.CommittedEvents.Select((e, revision) => new PublishedEvent
                {
                    AggregateId = stream.StreamId,
                    BucketId = stream.BucketId,
                    EventType = e.Body.GetType().FullName,
                    Revision = revision + 1,
                    Headers = AddRebuildHeaders(e.Headers, stream.StreamRevision),
                    Body = e.Body
                });

                await _eventPublisher.Publish(new PublishedEvents { 
                    AggregateId = stream.StreamId.ToString(),
                    BucketId = stream.BucketId,
                    Events = streamEvents.ToArray() });
            }
        }
    }
}