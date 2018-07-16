using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain;

namespace Swetugg.Tix.Activity.Jobs
{
    public class CommandDispatcher
    {
        public static Assembly CommandAssembly = typeof(CreateActivity).Assembly;

        private readonly DomainHost _domainHost;

        public CommandDispatcher(DomainHost domainHost)
        {
            _domainHost = domainHost;
        }

        public async Task DispatchCommand([ServiceBusTrigger("activitycommands")] Message commandMsg)
        {
            var messageType = CommandAssembly.GetType(commandMsg.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{commandMsg.Label}'");
            }

            var cmdString = Encoding.UTF8.GetString(commandMsg.Body);
            var command = JsonConvert.DeserializeObject(cmdString, messageType);

            Console.Out.WriteLine($"Dispatching {messageType.Name} command");
            _domainHost.Dispatcher.Dispatch(command);
            Console.Out.WriteLine("Command handled successfully");
        }

    }
}