using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public class CreateUserFunc : AuthorizedFunc<EmptyFuncParams>
    {
        private readonly IUserCommands _userCommands;

        public CreateUserFunc(IUserCommands userCommands, IAuthManager authManager) : base(authManager)
        {
            _userCommands = userCommands;
        }

        [FunctionName("CreateUser")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "me")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            // Get logged in user information
            var identity = req.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var subject = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var issuer = identity.FindFirst("iss").Value;

            var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (userInfo.UserId != null)
            {
                return new BadRequestResult();
            }

            userInfo.Subject = subject;
            userInfo.IssuerIdentifier = issuer;

            await _userCommands.CreateUser(userInfo);

            return new NoContentResult();
        }
    }
}
