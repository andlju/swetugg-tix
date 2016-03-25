using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Swetugg.Tix.Web.Options;

namespace Swetugg.Tix.Web
{
    public class CommandMessageSender : IMessageSender
    {
        private readonly string _queueName;
        private readonly string _serviceBusConnectionString;

        public CommandMessageSender(IOptions<StorageOptions> storageOptions, IOptions<MessagingOptions> messagingOptions)
        {
            _queueName = messagingOptions.Value.CommandDispatchQueue.QueueName;
            _serviceBusConnectionString = storageOptions.Value.AzureServiceBus.ConnectionString;

            var namespaceManager = NamespaceManager.CreateFromConnectionString(_serviceBusConnectionString);

            if (!namespaceManager.QueueExists(_queueName))
            {
                namespaceManager.CreateQueue(_queueName);
            }
        }

        public async Task Send(object message)
        {
            var client = QueueClient.CreateFromConnectionString(_serviceBusConnectionString, _queueName);

            await client.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(message)) { Label = message.GetType().FullName });
        }
    }
}