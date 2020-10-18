using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swetugg.Tix.Api.Options;
using System.Text;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Tickets.Commands
{
    public interface ITicketCommandMessageSender : IMessageSender
    {

    }

    public class TicketCommandMessageSender : ITicketCommandMessageSender
    {
        private readonly string _queueName;
        private readonly string _serviceBusConnectionString;
        private readonly QueueClient _client;

        public TicketCommandMessageSender(IOptions<ApiOptions> apiOptions)
        {
            _queueName = apiOptions.Value.TicketCommandsQueue;
            _serviceBusConnectionString = apiOptions.Value.TixServiceBus;
            _client = new QueueClient(_serviceBusConnectionString, _queueName);
        }

        public async Task Send(object body)
        {
            var byteBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            var message = new Message(byteBody)
            {
                Label = body.GetType().FullName
            };

            await _client.SendAsync(message);
        }

        public Task Close()
        {
            return _client.CloseAsync();
        }

    }
}