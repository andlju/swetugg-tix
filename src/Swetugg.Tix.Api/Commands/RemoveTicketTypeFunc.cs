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
    public class RemoveTicketTypeFunc : ActivityCommandFunc<RemoveTicketType>
    {
        public RemoveTicketTypeFunc(IMessageSender sender) : base(sender)
        {
        }

        [FunctionName("RemoveTicketType")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "activities/{activityId}/ticket-types/{ticketTypeId}")]
            HttpRequest req,
            Guid activityId,
            Guid ticketTypeId,
            ILogger log)
        {
            var cmd = await Process(req, new { activityId, ticketTypeId }, log);

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }
}
