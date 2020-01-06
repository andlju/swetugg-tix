using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Api
{
    public class CreateActivityFunc
    {
        private readonly IMessageSender _sender;

        public CreateActivityFunc(IMessageSender sender)
        {
            _sender = sender;
        }

        [FunctionName("CreateActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var createActivityCommand = new CreateActivity()
            {
                ActivityId = Guid.NewGuid(),
                CommandId = Guid.NewGuid()
            };

            await _sender.Send(createActivityCommand);

            return new OkObjectResult($"Creating Activity: {createActivityCommand.ActivityId}");
        }
    }
}
