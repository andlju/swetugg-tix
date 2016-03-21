using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Api.Controllers
{

    public class ConnectionStringOption
    {
        public string ConnectionString { get; set; }
    }
    public class StorageOptions
    {
        public ConnectionStringOption AzureWebJobsStorage { get; set; }
        public ConnectionStringOption AzureServiceBus { get; set; }
    }


    [Route("api/activity/commands")]
    public class ActivityCommandsController : Controller
    {
        private readonly StorageOptions _options;

        public ActivityCommandsController(IOptions<StorageOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet("test")]
        public async Task<string> Get()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_options.AzureServiceBus.ConnectionString);

            if (!namespaceManager.QueueExists("activitycommands"))
            {
                namespaceManager.CreateQueue("activitycommands");
            }

            var createActivityCommand = new CreateActivity()
            {
                ActivityId = Guid.NewGuid(),
                CommandId = Guid.NewGuid()
            };

            var client = QueueClient.CreateFromConnectionString(_options.AzureServiceBus.ConnectionString, "activitycommands");
            await client.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(createActivityCommand)));

            return "Command sent";
        }
    }
}