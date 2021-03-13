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
using Swetugg.Tix.Api.Options;
using Swetugg.Tix.User.Contract;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class GetUserFunc
    {
        private static string[] acceptedScopes = new[] { "access_as_user", "access_as_admin" };

        private readonly string _connectionString;

        public GetUserFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("GetUser")]
        // [RequiredScope("access_as_user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var (authenticationStatus, authenticationResponse) = await req.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            req.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);

            var identity = req.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var userId = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var issuer = identity.FindFirst("iss").Value;
            string name = req.HttpContext.User.Identity.IsAuthenticated ? req.HttpContext.User.Identity.Name : null;

            using (var conn = new SqlConnection(_connectionString))
            {
                var userInfo = (await conn.QueryFirstOrDefaultAsync<UserInfo>(
                    "SELECT u.UserId, u.Name, u.Status " +
                    "FROM [Users].[User] u JOIN [Users].[UserLogin] ul ON ul.UserId = u.UserId JOIN [Users].[Issuer] i ON i.IssuerId = ul.IssuerId " +
                    "WHERE ul.Subject = @Subject AND i.IssuerIdentifier = @Issuer AND u.Status <> @DeletedStatus",
                    new { Subject = userId, Issuer = issuer, DeletedStatus = UserStatus.Deleted }));
                if (userInfo != null)
                {
                    return new OkObjectResult(userInfo);
                }
            }

            return new OkObjectResult(new UserInfo { Name = name, Status = UserStatus.None });
        }
    }
}
