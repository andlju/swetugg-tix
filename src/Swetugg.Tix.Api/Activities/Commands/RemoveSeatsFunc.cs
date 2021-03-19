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
    public class RemoveSeatsFunc : ActivityCommandFunc<RemoveSeats>
    {
        public RemoveSeatsFunc(IActivityCommandMessageSender sender, IAuthManager authManager) : base(sender, authManager)
        {
        }

        [FunctionName("RemoveSeats")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities/{activityId}/remove-seats")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var (res, cmd) = await ProcessCommand(req, log, new { activityId });

            return res;
        }
    }
}
