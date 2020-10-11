using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Activity.Funcs.Options;
using Swetugg.Tix.Infrastructure;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Swetugg.Tix.Activity.Funcs
{

    public class ServiceBusPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private string _topicName;
        private string _serviceBusConnectionString;

        public ServiceBusPublisher(IOptions<ActivityOptions> activityOptions)
        {
            _topicName = activityOptions.Value.ActivityEventPublisherTopic;
            _serviceBusConnectionString = activityOptions.Value.TixServiceBus;
            _client = new TopicClient(_serviceBusConnectionString, _topicName);
        }

        public async Task Publish(PublishedEvents evts)
        {
            foreach(var evt in evts.Events)
            {
                var byteBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt.Body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                var message = new Message(byteBody)
                {
                    Label = evt.EventType
                };

                await _client.SendAsync(message);
            }
        }

        public Task Close()
        {
            return _client.CloseAsync();
        }
    }
}