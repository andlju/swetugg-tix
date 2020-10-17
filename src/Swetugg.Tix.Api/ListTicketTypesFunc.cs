using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Models;
using Swetugg.Tix.Api.Options;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api
{
    public class ListTicketTypesFunc
    {
        private readonly string _connectionString;
        public ListTicketTypesFunc(IMessageSender sender, IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("ListTicketTypes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/{activityId}/ticket-types")]
            HttpRequest req,
            string activityId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var conn = new SqlConnection(_connectionString))
            {
                var ticketTypes = (await conn.QueryAsync<TicketType>(
                    "SELECT a.ActivityId, a.Revision, tt.TicketTypeId, tt.Limit, tt.Reserved, ttc.Name " +
                    "FROM ActivityViews.ActivityOverview a " +
                    "JOIN ActivityViews.TicketType tt ON a.ActivityId = tt.ActivityId " +
                    "LEFT JOIN ActivityContent.TicketType ttc ON (tt.ActivityId = ttc.ActivityId AND tt.TicketTypeId = ttc.TicketTypeId) " +
                    "WHERE a.ActivityId = @activityId",
                    new { activityId })).ToArray();
                if (ticketTypes != null)
                {
                    var ticketTypesView = new TicketTypesView()
                    {
                        Revision = ticketTypes.FirstOrDefault()?.Revision ?? 0,
                        TicketTypes = ticketTypes.ToArray()
                    };
                    return new OkObjectResult(ticketTypesView);
                }
            }
            return new NotFoundResult();
        }
    }
}
