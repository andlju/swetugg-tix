﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class RemoveTicketTypeFunc : ActivityCommandFunc<RemoveTicketType>
    {
        public RemoveTicketTypeFunc(IActivityCommandMessageSender sender) : base(sender)
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
            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var cmd = await Process(req, new { activityId, ticketTypeId }, log);

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }

}
