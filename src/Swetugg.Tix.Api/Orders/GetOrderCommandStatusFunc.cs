using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Tickets
{
    public class GetOrderCommandStatusFunc
    {
        private readonly ICommandLog _commandLog;

        public GetOrderCommandStatusFunc(ICommandLog commandLog)
        {
            _commandLog = commandLog;
        }

        [FunctionName("GetOrderCommandStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/commands/{commandId}")]
            HttpRequest req,
            string commandId,
            ILogger log)
        {
            var commandGuid = Guid.Parse(commandId);
            var commandLog = await _commandLog.GetCommandLog(commandGuid);

            if (commandLog != null) { 
                return new OkObjectResult(commandLog);
            }
            return new NotFoundResult();
        }

    }
}
