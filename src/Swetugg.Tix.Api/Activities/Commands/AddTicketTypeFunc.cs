using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Content.Contract;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class AddTicketTypeFunc : ActivityCommandFunc<AddTicketType>
    {
        private readonly IActivityContentCommands _contentCommands;

        public AddTicketTypeFunc(IActivityCommandMessageSender sender, IActivityContentCommands contentCommands) : base(sender)
        {
            _contentCommands = contentCommands;
        }

        [FunctionName("AddTicketType")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities/{activityId}/ticket-types")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var ticketTypeId = Guid.NewGuid();
            var cmd = await Process(req, new { activityId, ticketTypeId }, log);
            await _contentCommands.StoreTicketTypeContent(new TicketTypeContent { TicketTypeId = ticketTypeId, ActivityId = activityId, Name = cmd.Name });

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }

}
