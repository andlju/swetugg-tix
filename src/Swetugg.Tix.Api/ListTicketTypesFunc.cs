using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Options;
using System.Data.SqlClient;
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
                var ticketTypes = await conn.QueryAsync<TicketType>(
                    "SELECT ActivityId, TicketTypeId, Limit, Reserved FROM ActivityViews.TicketType WHERE ActivityId = @activityId",
                    new { activityId });
                if (ticketTypes != null)
                {
                    return new OkObjectResult(ticketTypes);
                }
            }
            return new NotFoundResult();
        }
    }
}
