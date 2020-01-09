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
    public class GetActivityFunc
    {
        private readonly IMessageSender _sender;

        public GetActivityFunc(IMessageSender sender)
        {
            _sender = sender;
        }

        [FunctionName("GetActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity/{activityId}")] HttpRequest req,
            string activityId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(new
            {
                activityId,
                name = "An Activity"
            });
        } 
    }
}
