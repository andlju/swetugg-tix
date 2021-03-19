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
    public class CreateActivityFunc : ActivityCommandFunc<CreateActivity>
    {
        private readonly IActivityContentCommands _contentCommands;

        public CreateActivityFunc(IActivityCommandMessageSender sender, IActivityContentCommands contentCommands, Authorization.IAuthManager authManager) : base(sender, authManager)
        {
            _contentCommands = contentCommands;
        }

        [FunctionName("CreateActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activities")]
            HttpRequest req,
            ILogger log)
        {
            var activityId = Guid.NewGuid();
            var (res, cmd) = await ProcessCommand(req, log, new { activityId });
            
            await _contentCommands.StoreActivityContent(new ActivityContent { ActivityId = activityId, Name = cmd.Name });

            return res;
        }
    }
}
