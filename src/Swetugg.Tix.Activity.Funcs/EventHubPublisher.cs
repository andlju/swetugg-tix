using Swetugg.Tix.Infrastructure;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text.Json;
using Swetugg.Tix.Activity.Funcs.Options;
using Microsoft.Extensions.Options;

namespace Swetugg.Tix.Activity.Funcs
{
    public class EventHubPublisher : IEventPublisher
    {
        private string _eventHubConnectionString;
        private string _eventHubName;
        private EventHubProducerClient _client;

        public EventHubPublisher(IOptions<ActivityOptions> activityOptions)
        {
            _eventHubConnectionString = activityOptions.Value.EventHubConnectionString;
            _eventHubName = activityOptions.Value.ActivityEventHubName;
            _client = new EventHubProducerClient(_eventHubConnectionString, _eventHubName);
        }

        public async Task Publish(object evt, string aggregateId)
        {
            var batch = await _client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = aggregateId });
            var jsonBody = JsonSerializer.Serialize(evt, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            batch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonBody)));
            await _client.SendAsync(batch);
        }
    }
}