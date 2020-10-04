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
using System.Threading.Tasks;

namespace Swetugg.Tix.Api
{
    public class ListActivitiesFunc
    {
        private readonly string _connectionString;
        public ListActivitiesFunc(IOptions<ApiOptions> options)
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
                var activities = await conn.QueryAsync<ActivityOverview>("SELECT ActivityId, FreeSeats, TotalSeats, TicketTypes FROM ActivityViews.ActivityOverview");
                if (activities != null)
                {
                    return new OkObjectResult(activities);
                }
            }
            return new NotFoundResult();
        }
    }
}
