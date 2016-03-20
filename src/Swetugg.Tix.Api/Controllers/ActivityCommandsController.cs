using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Api.Controllers
{
    public class StorageOptions
    {
        public string AzureWebJobsStorage { get; set; }
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
            var storageAccount = CloudStorageAccount.Parse(_options.AzureWebJobsStorage);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            var activityCommandsQueue = queueClient.GetQueueReference("activitycommands");
            activityCommandsQueue.CreateIfNotExists();

            var createActivityCommand = new CreateActivity()
            {
                ActivityId = Guid.NewGuid(),
                CommandId = Guid.NewGuid()
            };

            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(createActivityCommand));
            await activityCommandsQueue.AddMessageAsync(queueMessage);

            return "Hello World";
        }
    }
}