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

        public async Task Publish(object evt, string aggregateId)
        {
            var byteBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            var message = new Message(byteBody)
            {
                Label = evt.GetType().FullName
            };

            await _client.SendAsync(message);
        }

        public Task Close()
        {
            return _client.CloseAsync();
        }
    }
}