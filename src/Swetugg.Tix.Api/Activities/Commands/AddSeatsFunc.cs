using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class AddSeatsFunc : ActivityCommandFunc<AddSeats>
    {
        public AddSeatsFunc(IActivityCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("AddSeats")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities/{activityId}/add-seats")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var cmd = await Process(req, new { activityId }, log);

            return new OkObjectResult(new { activityId, commandId = cmd.CommandId });
        }
    }

}
