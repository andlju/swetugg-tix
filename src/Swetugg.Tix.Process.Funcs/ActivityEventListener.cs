using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process.Funcs
{
    public class ActivityEventListenerFunc
    {
        public static Assembly ActivityEventAssembly = typeof(Swetugg.Tix.Activity.Events.ActivityCreated).Assembly;
        private readonly JsonSerializerOptions _jsonOptions;

        private readonly ProcessHost _processHost;
        private readonly ILogger _logger;

        public ActivityEventListenerFunc(ProcessHost processHost, ILogger<ActivityEventListenerFunc> logger)
        {
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new PublishedEventConverter(typeof(Activity.Events.EventBase).Assembly));
            _processHost = processHost;
            _logger = logger;
        }

        [FunctionName("HandleActivityEvents")]
        public async Task Run([EventHubTrigger("activity", Connection = "EventHubConnectionString", ConsumerGroup = "activityproc")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var evt = JsonSerializer.Deserialize<PublishedEvent>(messageBody, _jsonOptions);

                    // Don't process if this is a rebuild
                    if (evt.Headers != null && evt.Headers.ContainsKey("RebuildToRevision"))
                        continue;

                    _logger.LogInformation($"Processing {evt.EventType}");
                    await _processHost.Dispatcher.Dispatch(evt.Body, false);
                    _logger.LogInformation($"Processing {evt.EventType} Completed");
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