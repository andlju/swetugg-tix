using System;
using System.Reflection;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain;

namespace Swetugg.Tix.Activity.Funcs
{
    public class ActivityCommandListenerFunc
    {
        public static Assembly CommandAssembly = typeof(CreateActivity).Assembly;

        private readonly DomainHost _domainHost;

        public ActivityCommandListenerFunc(DomainHost domainHost)
        {
            _domainHost = domainHost;
        }

        [FunctionName("ActivityCommandListenerFunc")]
        public void Run([ServiceBusTrigger("%ActivityCommandsQueue%", Connection = "TixServiceBus")]Message commandMsg, ILogger log)
        {
            var messageType = CommandAssembly.GetType(commandMsg.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{commandMsg.Label}'");
            }

            var cmdString = Encoding.UTF8.GetString(commandMsg.Body);
            var command = JsonConvert.DeserializeObject(cmdString, messageType);

            _domainHost.Dispatcher.Dispatch(command);
            Console.Out.WriteLine($"{messageType.Name} Command handled successfully");
        }
    }
}
