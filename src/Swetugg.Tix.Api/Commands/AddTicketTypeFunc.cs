using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Api.Commands
{
    public class AddTicketTypeFunc : ActivityCommandFunc<AddTicketType>
    {
        public AddTicketTypeFunc(IMessageSender sender) : base(sender)
        {
        }

        [FunctionName("AddTicketType")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities/{activityId}/ticket-types")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var ticketTypeId = Guid.NewGuid();
            var cmd = await Process(req, new { activityId, ticketTypeId }, log);

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }
    
}
