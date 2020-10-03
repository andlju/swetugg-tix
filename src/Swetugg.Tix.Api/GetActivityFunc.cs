using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dapper;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Api.Options;
using Microsoft.Extensions.Options;

namespace Swetugg.Tix.Api
{

    public class GetActivityFunc
    {
        private readonly string _connectionString;
        public GetActivityFunc(IMessageSender sender, IOptions<ApiOptions> options)
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
                var activity = await conn.QuerySingleOrDefaultAsync<ActivityOverview>("SELECT ActivityId, FreeSeats, TotalSeats, TicketTypes FROM ActivityViews.ActivityOverview WHERE ActivityId = @ActivityId", new { activityId });
                if (activity != null)
                {
                    return new OkObjectResult(activity);
                }
            }
            return new NotFoundResult();
        }
    }
}
