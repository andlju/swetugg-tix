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

namespace Swetugg.Tix.Api.Activities
{

    public class GetActivityFunc
    {
        private readonly string _connectionString;
        public GetActivityFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("GetActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/{activityId}")]
            HttpRequest req,
            string activityId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var conn = new SqlConnection(_connectionString))
            {
                var activity = await conn.QuerySingleOrDefaultAsync<ActivityOverview>(
                    "SELECT ao.ActivityId, ao.Revision, ao.FreeSeats, ao.TotalSeats, ao.TicketTypes, ac.Name " +
                    "FROM ActivityViews.ActivityOverview ao LEFT JOIN ActivityContent.Activity ac ON ao.ActivityId = ac.ActivityId " +
                    "WHERE ao.ActivityId = @ActivityId",
                    new { activityId });
                if (activity != null)
                {
                    return new OkObjectResult(activity);
                }
            }
            return new NotFoundResult();
        }
    }
}
