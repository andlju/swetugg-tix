using Microsoft.AspNetCore.Http;
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
    public class RemoveSeatsFunc : ActivityCommandFunc<RemoveSeats>
    {
        public RemoveSeatsFunc(IActivityCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("RemoveSeats")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities/{activityId}/remove-seats")]
            HttpRequest req,
            Guid activityId,
            ILogger log)
        {
            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var cmd = await Process(req, new { activityId }, log);

            return new OkObjectResult(new { activityId, commandId = cmd.CommandId });
        }
    }
}
