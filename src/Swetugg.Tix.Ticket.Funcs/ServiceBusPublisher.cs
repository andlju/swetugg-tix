using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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