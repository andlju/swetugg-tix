using NEventStore;
using Swetugg.Tix.Order.Commands.Admin;
using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Domain.Handlers.Admin
{
    public class RebuildAllViewsHandler : AdminCommandHandler<RebuildAllViews>
    {
        private readonly IStoreEvents _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public RebuildAllViewsHandler(IStoreEvents eventStore, IEventPublisher eventPublisher, ICommandLog commandLog) : base(commandLog)
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

        protected async override Task HandleCommand(RebuildAllViews command)
        {
            var commits = _eventStore.Advanced.GetFrom(0);
            foreach(var commit in commits)
            {
                var initialRevision = commit.StreamRevision - commit.Events.Count;
                var streamEvents = commit.Events.Select((e, revision) => new PublishedEvent
                {
                    AggregateId = commit.StreamId,
                    BucketId = commit.BucketId,
                    EventType = e.Body.GetType().FullName,
                    Revision = initialRevision + revision + 1,
                    Headers = AddRebuildHeaders(e.Headers, commit.StreamRevision),
                    Body = e.Body
                });

                await _eventPublisher.Publish(new PublishedEvents { 
                    AggregateId = commit.StreamId, 
                    BucketId = commit.BucketId,
                    Events = streamEvents.ToArray()
                });
            }
        }
    }
}