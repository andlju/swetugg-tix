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
    public class EventListenerFunc
    {
        public static Assembly ActivityEventAssembly = typeof(Swetugg.Tix.Activity.Events.ActivityCreated).Assembly;

        private readonly ProcessHost _processHost;
        private readonly ILogger _logger;

        public EventListenerFunc(ProcessHost processHost, ILogger<EventListenerFunc> logger)
        {
            _processHost = processHost;
            _logger = logger;
        }

        [FunctionName("HandleActivityEvent")]
        public async Task Run([ServiceBusTrigger("%ActivityEventPublisherTopic%", "%ProcessActivitySub%", Connection = "TixServiceBus")] Message eventMessage)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {eventMessage.Label}");
            var messageType = ActivityEventAssembly.GetType(eventMessage.Label, false);
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
