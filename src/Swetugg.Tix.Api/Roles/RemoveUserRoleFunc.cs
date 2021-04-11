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
using Swetugg.Tix.Organization;
using Swetugg.Tix.Organization.Contract;
using Swetugg.Tix.User;
using Swetugg.Tix.User.Contract;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Api.Roles
{
    public class RemoveUserRoleFuncParams
    {
        public Guid UserId { get; set; }
        public Guid UserRoleId { get; set; }

    }

    public class RemoveUserRoleFunc : AuthorizedFunc<RemoveUserRoleFuncParams>
    {

        private readonly IUserAuthorizationService _userAuthorizationService;

        public RemoveUserRoleFunc(IAuthManager authManager, IUserAuthorizationService userAuthorizationService) : base(authManager)
        {
            _userAuthorizationService = userAuthorizationService;
        }

        [FunctionName("RemoveUserRole")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{userId}/roles/{userRoleId}")]
            HttpRequest req,
            ILogger log,
            Guid userId,
            Guid userRoleId)
        {
            return Process(req, log, new RemoveUserRoleFuncParams { UserId = userId, UserRoleId = userRoleId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, RemoveUserRoleFuncParams funcParams)
        {
            var user = await AuthManager.GetAuthorizedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            await _userAuthorizationService.RemoveUserRole(funcParams.UserId, funcParams.UserRoleId);
            
            return new NoContentResult();
        }
    }
}
