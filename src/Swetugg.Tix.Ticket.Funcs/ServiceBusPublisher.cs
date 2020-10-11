using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Funcs.Options;
using System.Text;
using System.Threading.Tasks;

namespace Swetugg.Tix.Ticket.Funcs
{
    public class ServiceBusPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private string _topicName;
        private string _serviceBusConnectionString;

        public ServiceBusPublisher(IOptions<TicketOptions> ticketOptions)
        {
            _topicName = ticketOptions.Value.TicketEventPublisherTopic;
            _serviceBusConnectionString = ticketOptions.Value.TixServiceBus;
            _client = new TopicClient(_serviceBusConnectionString, _topicName);
        }

        public async Task Publish(PublishedEvents evts)
        {
            foreach (var evt in evts.Events)
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