using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Api.Authorization;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class CommandStatusRouteParams
    {
        public string CommandId { get; set; }
    }

    public class GetActivityCommandStatusFunc : AuthorizedFunc<CommandStatusRouteParams>
    {
        private static string[] acceptedScopes = new[] { "access_as_admin" };
        private readonly ICommandLog _commandLog;

        public GetActivityCommandStatusFunc(ICommandLog commandLog, IAuthManager authManager): base(authManager)
        {
            _commandLog = commandLog;
        }

        [FunctionName("GetActivityCommandStatus")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/commands/{commandId}")]
            HttpRequest req,
            string commandId,
            ILogger log)
        {
            return Process(req, log, new CommandStatusRouteParams { CommandId = commandId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, CommandStatusRouteParams routeParams)
        {
            var commandGuid = Guid.Parse(routeParams.CommandId);
            var commandLog = await _commandLog.GetCommandLog(commandGuid);

            if (commandLog != null)
            {
                return new OkObjectResult(commandLog);
            }
            return new NotFoundResult();
        }
    }
}
