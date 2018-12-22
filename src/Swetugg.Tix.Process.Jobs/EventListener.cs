using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Swetugg.Tix.Process.Jobs
{
    public class EventListener
    {
        public static Assembly ActivityEventAssembly = typeof(Swetugg.Tix.Activity.Events.ActivityCreated).Assembly;

        private readonly ProcessHost _processHost;
        private readonly ILogger _logger;

        public EventListener(ProcessHost processHost, ILogger<EventListener> logger)
        {
            _processHost = processHost;
            _logger = logger;
        }

        public async Task HandleActivityEvent([ServiceBusTrigger("activityevents", "tixprocess", Connection = "ServiceBus")] Message eventMessage)
        {
            var messageType = ActivityEventAssembly.GetType(eventMessage.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{eventMessage.Label}'");
            }

            var evtString = Encoding.UTF8.GetString(eventMessage.Body);
            var evt = JsonConvert.DeserializeObject(evtString, messageType);

            _processHost.Dispatcher.Dispatch(evt, false);
        }

    }
}