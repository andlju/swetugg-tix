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
using Swetugg.Tix.User;
using Swetugg.Tix.User.Contract;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class SetUserFunc
    {
        private static string[] acceptedScopes = new[] { "access_as_user", "access_as_admin" };

        private readonly IUserCommands _userCommands;

        public SetUserFunc(IOptions<ApiOptions> options, IUserCommands userCommands)
        {
            _userCommands = userCommands;
        }

        [FunctionName("SetUser")]
        // [RequiredScope("access_as_user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "me")]
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

            var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            userInfo.Subject = userId;
            userInfo.IssuerIdentifier = issuer;

            if (userInfo.UserId != null)
            {
                await _userCommands.SetUser(userInfo);
            } 
            else
            {
                await _userCommands.CreateUser(userInfo);
            }

            return new NoContentResult();
        }
    }
}
