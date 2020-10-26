using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands.Admin;
using Swetugg.Tix.Api.Activities.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Admin
{

    public class RebuildViewsFunc : ActivityAdminCommandFunc<RebuildViews>
    {
        public RebuildViewsFunc(IActivityCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("RebuildViews")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities-admin/{activityId}/rebuild")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var cmd = await Process(req, new { activityId }, log);

            return new OkObjectResult(new { activityId, commandId = cmd.CommandId });
        }
    }
}
