﻿using Dapper;
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
using Swetugg.Tix.Api.Options;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class ListActivitiesFunc
    {
        private static string[] acceptedScopes = new[] { "access_as_user", "access_as_admin" };

        private readonly string _connectionString;
        private readonly TableStorageViewReader _viewReader;

        public ListActivitiesFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
            _viewReader = new TableStorageViewReader(options.Value.AzureWebJobsStorage, "activityview");
        }

        [FunctionName("ListActivities")]
        // [RequiredScope("access_as_user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activities")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var identity = req.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var userId = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            string name = req.HttpContext.User.Identity.IsAuthenticated ? req.HttpContext.User.Identity.Name : null;

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
