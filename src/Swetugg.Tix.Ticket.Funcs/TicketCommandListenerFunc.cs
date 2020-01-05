using System;
using System.Reflection;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swetugg.Tix.Ticket.Commands;
using Swetugg.Tix.Ticket.Domain;

namespace Swetugg.Tix.Ticket.Funcs
{
    public class TicketCommandListenerFunc
    {
        public static Assembly CommandAssembly = typeof(CreateTicket).Assembly;

        private readonly DomainHost _domainHost;

        public TicketCommandListenerFunc(DomainHost domainHost)
        {
            _domainHost = domainHost;
        }

        [FunctionName("TicketCommandListenerFunc")]
        public void Run([ServiceBusTrigger("ticketcommands", Connection = "TixServiceBus")]Message commandMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {commandMsg.Label}");
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
