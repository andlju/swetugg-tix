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
    public class AddSeatsFunc : ActivityCommandFunc<AddSeats>
    {
        public AddSeatsFunc(IMessageSender sender) : base(sender)
        {
        }

        [FunctionName("AddSeats")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activity/{activityId}/add-seats")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var cmd = await Process(req, new { activityId }, log);

            return new OkObjectResult(new { activityId, commandId = cmd.CommandId });
        }
    }
}
