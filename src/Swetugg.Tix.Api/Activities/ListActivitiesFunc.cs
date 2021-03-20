using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Api.Authorization;
using Swetugg.Tix.Api.Options;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class ListActivitiesFunc : AuthorizedFunc<EmptyFuncParams>
    {
        private readonly string _connectionString;
        private readonly TableStorageViewReader _viewReader;

        public ListActivitiesFunc(IOptions<ApiOptions> options, IAuthManager authManager) : base(authManager)
        {
            _connectionString = options.Value.ViewsDbConnection;
            _viewReader = new TableStorageViewReader(options.Value.AzureWebJobsStorage, "activityview");
        }

        [FunctionName("ListActivities")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams routeParams)
        {
            var user = await AuthManager.GetAuthenticatedUser();
            req.Query.TryGetValue("OwnerId", out var ownerIdQuery);
            var ownerId = string.IsNullOrEmpty(ownerIdQuery) ? user.UserId.ToString() : ownerIdQuery.ToString();

            var activities = (await _viewReader.ListEntitiesForPartition<ActivityViewEntity, ActivityOverview>(ownerId)).ToArray();

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
