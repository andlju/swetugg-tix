using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Swetugg.Tix.Process.Jobs
{
    public class EventListener
    {
        public static Assembly ActivityEventAssembly = typeof(Swetugg.Tix.Activity.Events.ActivityCreated).Assembly;

        private readonly ProcessHost _processHost;

        public EventListener(ProcessHost processHost)
        {
            _processHost = processHost;
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

            Console.Out.WriteLine($"Dispatching {messageType.Name} event to saga host");
            _processHost.Dispatcher.Dispatch(evt);
            Console.Out.WriteLine("Event handled successfully");
        }

    }
}