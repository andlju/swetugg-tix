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
using Swetugg.Tix.User;
using Swetugg.Tix.User.Contract;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Users
{
    public class SetUserFunc : AuthorizedFunc<EmptyFuncParams>
    {
        private readonly IUserCommands _userCommands;

        public SetUserFunc(IUserCommands userCommands, IAuthManager authManager): base(authManager)
        {
            _userCommands = userCommands;
        }

        [FunctionName("SetUser")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "me")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            var identity = req.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var subject = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var issuer = identity.FindFirst("iss").Value;

            var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            userInfo.Subject = subject;
            userInfo.IssuerIdentifier = issuer;

            if (userInfo.UserId == null)
            {
                return new BadRequestResult();
            }

            await _userCommands.SetUser(userInfo);

            return new NoContentResult();
        }
    }
}
