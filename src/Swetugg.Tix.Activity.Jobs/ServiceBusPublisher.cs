using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Domain;
using Swetugg.Tix.Activity.Jobs.Options;

namespace Swetugg.Tix.Activity.Jobs
{
    public class ServiceBusPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private string _topicName;
        private string _serviceBusConnectionString;

        public ServiceBusPublisher(IOptions<StorageOptions> storageOptions, IOptions<MessagingOptions> messagingOptions)
        {
            _topicName = messagingOptions.Value.EventPublisherTopic.TopicName;
            _serviceBusConnectionString = storageOptions.Value.AzureServiceBus.ConnectionString;
            _client = new TopicClient(_serviceBusConnectionString, _topicName);
        }

        public async Task Publish(object evt)
        {
            var byteBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evt));
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