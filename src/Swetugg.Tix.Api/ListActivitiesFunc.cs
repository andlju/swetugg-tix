using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dapper;
using Swetugg.Tix.Api.Options;
using Microsoft.Extensions.Options;

namespace Swetugg.Tix.Api
{
    public class ListActivitiesFunc
    {
        private readonly string _connectionString;
        public ListActivitiesFunc(IMessageSender sender, IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("ListActivities")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var conn = new SqlConnection(_connectionString))
            {
                var activities = await conn.QueryAsync<ActivityOverview>("SELECT ActivityId, Name, FreeSeats, TotalSeats, TicketTypes FROM ActivityOverview");
                if (activities != null)
                {
                    return new OkObjectResult(activities);
                }
            }
            return new NotFoundResult();
        }
    }
}
