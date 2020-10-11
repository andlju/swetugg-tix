using Swetugg.Tix.Infrastructure;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text.Json;
using Swetugg.Tix.Activity.Funcs.Options;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Generic;

namespace Swetugg.Tix.Activity.Funcs
{
    public class EventHubPublisher : IEventPublisher
    {
        private string _eventHubConnectionString;
        private string _eventHubName;
        private EventHubProducerClient _client;
        private JsonSerializerOptions _jsonOptions;

        public EventHubPublisher(IOptions<ActivityOptions> activityOptions)
        {
            _eventHubConnectionString = activityOptions.Value.EventHubConnectionString;
            _eventHubName = activityOptions.Value.ActivityEventHubName;
            _client = new EventHubProducerClient(_eventHubConnectionString, _eventHubName);
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _jsonOptions.Converters.Add(new PublishedEventConverter(typeof(Activity.Events.EventBase).Assembly));
        }

        public async Task Publish(PublishedEvents evts)
        {
            var batch = await _client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = evts.AggregateId });
            foreach(var evt in evts.Events)
            {
                var jsonBody = JsonSerializer.Serialize(evt, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                batch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonBody)));
            }
            await _client.SendAsync(batch);
        }
    }
}