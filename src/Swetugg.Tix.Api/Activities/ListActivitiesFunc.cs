using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Api.Auth;
using Swetugg.Tix.Api.Options;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class ListActivitiesFunc
    {
        private readonly string _connectionString;
        private readonly TableStorageViewReader _viewReader;
        private readonly JwtBearerValidator _validator;

        public ListActivitiesFunc(IOptions<ApiOptions> options, JwtBearerValidator validator)
        {
            _connectionString = options.Value.ViewsDbConnection;
            _viewReader = new TableStorageViewReader(options.Value.AzureWebJobsStorage, "activityview");
            _validator = validator;
        }

        [FunctionName("ListActivities")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var principal = await _validator.ValidateTokenAsync(req.Headers["Authorization"]);
            if (principal == null)
                return new UnauthorizedResult();

            var activities = (await _viewReader.ListAllEntities<ActivityViewEntity, ActivityOverview>()).ToArray();

            using (var conn = new SqlConnection(_connectionString))
            {
                var activityContent = (await conn.QueryAsync<ActivityContent>(
                    "SELECT ac.ActivityId, ac.Name " +
                    "FROM ActivityContent.Activity ac " +
                    "WHERE ac.ActivityId IN @ActivityIds",
                    new { ActivityIds = activities.Select(a => a.ActivityId).ToArray() })).ToArray();

                foreach (var a in activities)
                {
                    a.Name = activityContent.FirstOrDefault(ac => ac.ActivityId == a.ActivityId)?.Name;
                }
            }
            return new OkObjectResult(activities);
        }
    }
}
