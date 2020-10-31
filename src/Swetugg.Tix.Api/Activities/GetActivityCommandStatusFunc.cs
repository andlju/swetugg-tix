using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class GetActivityCommandStatusFunc
    {
        private readonly ICommandLog _commandLog;

        public GetActivityCommandStatusFunc(ICommandLog commandLog)
        {
            _commandLog = commandLog;
        }

        [FunctionName("GetActivityCommandStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/commands/{commandId}")]
            HttpRequest req,
            string commandId,
            ILogger log)
        {
            var commandGuid = Guid.Parse(commandId);
            var commandLog = await _commandLog.GetCommandLog(commandGuid);

            if (commandLog != null)
            {
                return new OkObjectResult(commandLog);
            }
            return new NotFoundResult();
        }

    }
}
