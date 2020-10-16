using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Models;
using Swetugg.Tix.Api.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api
{
    public class GetActivityCommandStatusFunc
    {
        private readonly string _connectionString;
        public GetActivityCommandStatusFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("GetActivityCommandStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/commands/{commandId}")]
            HttpRequest req,
            string commandId,
            ILogger log)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var lookup = new Dictionary<Guid, CommandLog>();
                var commandLogs = await conn.QueryAsync<CommandLog, CommandLogMessage, CommandLog>(
                    "SELECT cl.CommandId, cl.AggregateId, cl.Status, cl.CommandType, cl.JsonBody, cl.LastUpdated, clm.Id, clm.Code, clm.Message, clm.Severity, clm.Timestamp " +
                    "FROM ActivityLogs.CommandLog cl " +
                    "LEFT JOIN ActivityLogs.CommandLogMessage clm ON cl.CommandId = clm.CommandId " +
                    "WHERE cl.CommandId = @CommandId",
                    (cl, clm) =>
                    {
                        if (!lookup.TryGetValue(cl.CommandId, out var commandLog))
                            lookup.Add(cl.CommandId, commandLog = cl);
                        if (commandLog.Messages == null)
                            commandLog.Messages = new List<CommandLogMessage>();
                        if (clm?.Code != null)
                        {
                            commandLog.Messages.Add(clm);
                        }
                        return commandLog;
                    }
                    , new { commandId });

                var firstLog = commandLogs.FirstOrDefault();
                if (firstLog != null)
                {
                    return new OkObjectResult(firstLog);
                }
            }
            return new NotFoundResult();
        }

    }
}
