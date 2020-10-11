using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public class PublishedEvents
    {
        public string AggregateId { get; set; }
        public IEnumerable<PublishedEvent> Events { get; set; }
    }

    public class PublishedEvent
    {
        public string EventType { get; set; }
        public object Body { get; set; }
        public IEnumerable<KeyValuePair<string, object>> Headers { get; set; }
    }


    public interface IEventPublisher
    {
        Task Publish(PublishedEvents evts);
    }
}