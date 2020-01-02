using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swetugg.Tix.Web.Options;

namespace Swetugg.Tix.Web
{
    public class CommandMessageSender : IMessageSender
    {
        private readonly string _queueName;
        private readonly string _serviceBusConnectionString;
        private readonly QueueClient _client;

        public CommandMessageSender(IOptions<StorageOptions> storageOptions, IOptions<MessagingOptions> messagingOptions)
        {
            _queueName = messagingOptions.Value.CommandDispatchQueue.QueueName;
            _serviceBusConnectionString = storageOptions.Value.AzureServiceBus.ConnectionString;
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