using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Domain;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Funcs
{
    public class OrderCommandListenerFunc
    {
        public static Assembly CommandAssembly = typeof(CreateOrder).Assembly;

        private readonly DomainHost _domainHost;

        public OrderCommandListenerFunc(DomainHost domainHost)
        {
            _domainHost = domainHost;
        }

        [FunctionName("OrderCommandListenerFunc")]
        public async Task Run([ServiceBusTrigger("ordercommands", Connection = "TixServiceBus")] Message commandMsg, ILogger log)
        {
            var messageType = CommandAssembly.GetType(commandMsg.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{commandMsg.Label}'");
            }

            var cmdString = Encoding.UTF8.GetString(commandMsg.Body);
            var command = JsonConvert.DeserializeObject(cmdString, messageType);

            await _domainHost.Dispatcher.Dispatch(command);
        }
    }
}
