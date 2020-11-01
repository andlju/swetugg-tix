﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class RemoveTicketTypeLimitFunc : ActivityCommandFunc<RemoveTicketTypeLimit>
    {
        public RemoveTicketTypeLimitFunc(IActivityCommandMessageSender sender) : base(sender)
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
            var cmd = await Process(req, new { activityId, ticketTypeId }, log);

            return new OkObjectResult(new { activityId, ticketTypeId, commandId = cmd.CommandId });
        }
    }

}