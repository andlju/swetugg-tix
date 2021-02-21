using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Api.Options;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{

    public class GetActivityFunc
    {
        private static string[] acceptedScopes = new[] { "access_as_user", "access_as_admin" };

        private readonly string _connectionString;
        private TableStorageViewReader _viewReader;

        public GetActivityFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
            // TODO Use Singleton
            _viewReader = new TableStorageViewReader(options.Value.AzureWebJobsStorage, "activityview");
        }

        [FunctionName("GetActivity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities/{activityId}")]
            HttpRequest req,
            string activityId,
            ILogger log)
        {
            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var activity = await _viewReader.GetEntity<ActivityViewEntity, ActivityOverview>(activityId, activityId);
            if (activity == null)
            {
                activity = new ActivityOverview()
                {
                    ActivityId = Guid.Parse(activityId),
                    Revision = -1,
                };
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                var activityContent = await conn.QuerySingleAsync<ActivityContent>(
                    "SELECT ac.ActivityId, ac.Name " +
                    "FROM ActivityContent.Activity ac " +
                    "WHERE ac.ActivityId = @ActivityId",
                    new { activityId });
                if (activityContent != null)
                {
                    activity.Name = activityContent.Name;
                    if (activity.Revision < 0) 
                        activity.Revision = 0;
                }
                var ticketTypeNames = (await conn.QueryAsync<TicketTypeContent>(
                    "SELECT tt.ActivityId, tt.TicketTypeId, tt.Name " +
                    "FROM ActivityContent.TicketType tt " +
                    "WHERE tt.ActivityId = @ActivityId",
                    new { activityId })).ToArray();

                foreach(var tt in activity.TicketTypes)
                {
                    tt.Name = ticketTypeNames.FirstOrDefault(tn => tn.TicketTypeId == tt.TicketTypeId)?.Name;
                }
            }
            if (activity.Revision >= 0)
                return new OkObjectResult(activity);

            return new NotFoundResult();
        }
    }
}
