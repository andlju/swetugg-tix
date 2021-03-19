using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Api.Authorization;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public class AddTicketTypeFunc : ActivityCommandFunc<AddTicketType>
    {
        private readonly IActivityContentCommands _contentCommands;

        public AddTicketTypeFunc(IActivityCommandMessageSender sender, IActivityContentCommands contentCommands, IAuthManager authManager) : base(sender, authManager)
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
            var ticketTypeId = Guid.NewGuid();
            var (res, cmd) = await ProcessCommand(req, log, new { activityId, ticketTypeId });
            await _contentCommands.StoreTicketTypeContent(new TicketTypeContent { TicketTypeId = ticketTypeId, ActivityId = activityId, Name = cmd.Name });

            return res;
        }
    }

}
