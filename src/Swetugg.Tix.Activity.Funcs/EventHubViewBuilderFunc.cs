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
using Polly;
using Swetugg.Tix.Activity.Events;
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
            _jsonOptions.Converters.Add(new PublishedEventConverter(typeof(EventBase).Assembly));
            _host = host;
        }

        [FunctionName("EventHubViewBuilderFunc")]
        public async Task Run([EventHubTrigger("activity", Connection = "EventHubConnectionString", ConsumerGroup = "activityviews")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            var publishedEvents = events.Select(eventData =>
            {
                string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var evt = JsonSerializer.Deserialize<PublishedEvent>(messageBody, _jsonOptions);
                return evt;
            });

            await _host.HandlePublishedEvents(publishedEvents);
        }
    }
}
