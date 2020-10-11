using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.ViewBuilder;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Funcs
{

    public class EventHubViewBuilderFunc
    {
        private readonly JsonSerializerOptions _jsonOptions;

        private readonly ViewBuilderHost _host;

        public EventHubViewBuilderFunc(ViewBuilderHost host)
        {
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new PublishedEventConverter(typeof(Activity.Events.EventBase).Assembly));
            _host = host;
        }

        [FunctionName("EventHubViewBuilderFunc")]
        public async Task Run([EventHubTrigger("%ActivityEventHubName%", Connection = "EventHubConnectionString", ConsumerGroup = "%ActivityViewsConsumerGroup%")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var evt = JsonSerializer.Deserialize<PublishedEvent>(messageBody, _jsonOptions);
                    // Replace these two lines with your processing logic.
                    log.LogInformation($"Processing {evt.EventType} Event");
                    await _host.HandlePublishedEvent(evt);
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
