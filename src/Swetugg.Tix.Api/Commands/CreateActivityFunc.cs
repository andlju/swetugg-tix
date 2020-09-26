using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Api.Commands
{
    public class CreateActivityFunc : ActivityCommandFunc<CreateActivity>
    {

        public CreateActivityFunc(IMessageSender sender) : base(sender)
        {

        }

        [FunctionName("CreateActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activity")]
            HttpRequest req,
            ILogger log)
        {
            var activityId = Guid.NewGuid();
            var cmd = await Process(req, new { activityId }, log);

            return new OkObjectResult(new { activityId, commandId = cmd.CommandId });
        }
    }
}
