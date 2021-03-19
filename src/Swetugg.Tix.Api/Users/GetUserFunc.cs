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
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class GetUserFunc: AuthorizedFunc<EmptyFuncParams>
    {
        private readonly IUserQueries _userQueries;

        public GetUserFunc(IUserQueries userQueries, IAuthManager authManager) : base(authManager)
        {
            _userQueries = userQueries;
        }

        [FunctionName("GetUser")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")]
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
            string name = req.HttpContext.User.Identity.IsAuthenticated ? req.HttpContext.User.Identity.Name : null;

            var userInfo = await _userQueries.GetUserFromLogin(subject, issuer);
            if (userInfo != null)
            {
                return new OkObjectResult(userInfo);
            }

            return new OkObjectResult(new UserInfo { Name = name, Status = UserStatus.None });
        }
    }
}
