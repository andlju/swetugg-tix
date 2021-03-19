using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Api.Authorization;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class RemoveTicketTypeLimitFunc : ActivityCommandFunc<RemoveTicketTypeLimit>
    {
        public RemoveTicketTypeLimitFunc(IActivityCommandMessageSender sender, IAuthManager authManager) : base(sender, authManager)
        {
        }

        [FunctionName("RemoveTicketTypeLimit")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "activities/{activityId}/ticket-types/{ticketTypeId}/limit")]
            HttpRequest req,
            Guid activityId,
            Guid ticketTypeId,
            ILogger log)
        {
            var (res, cmd) = await ProcessCommand(req, log, new { activityId, ticketTypeId });

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }

}
