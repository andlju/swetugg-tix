using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process.Funcs
{
    public class TicketEventListenerFunc
    {
        public static Assembly TicketEventAssembly = typeof(Swetugg.Tix.Ticket.Events.TicketCreated).Assembly;

        private readonly ProcessHost _processHost;
        private readonly ILogger _logger;

        public TicketEventListenerFunc(ProcessHost processHost, ILogger<TicketEventListenerFunc> logger)
        {
            _processHost = processHost;
            _logger = logger;
        }

        [FunctionName("HandleTicketEvent")]
        public async Task Run([ServiceBusTrigger("%TicketEventPublisherTopic%", "%ProcessTicketSub%", Connection = "TixServiceBus")] Message eventMessage)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {eventMessage.Label}");
            var messageType = TicketEventAssembly.GetType(eventMessage.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{eventMessage.Label}'");
            }

            var evtString = Encoding.UTF8.GetString(eventMessage.Body);
            var evt = JsonConvert.DeserializeObject(evtString, messageType);

            await _processHost.Dispatcher.Dispatch(evt, false);
        }

    }
}
