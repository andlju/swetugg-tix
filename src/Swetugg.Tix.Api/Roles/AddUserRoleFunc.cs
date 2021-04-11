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
    public class AddUserRoleFuncParams
    {
        public Guid UserId { get; set; }

    }

    public class AddUserRoleFunc : AuthorizedFunc<AddUserRoleFuncParams>
    {

        private readonly IUserAuthorizationService _userAuthorizationService;

        public AddUserRoleFunc(IAuthManager authManager, IUserAuthorizationService userAuthorizationService) : base(authManager)
        {
            _userAuthorizationService = userAuthorizationService;
        }

        [FunctionName("AddUserRole")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{userId}/roles")]
            HttpRequest req,
            ILogger log,
            Guid userId)
        {
            return Process(req, log, new AddUserRoleFuncParams { UserId = userId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, AddUserRoleFuncParams funcParams)
        {
            var userRole = await JsonSerializer.DeserializeAsync<UserRole>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (userRole.UserRoleId != Guid.Empty)
                return new BadRequestResult();

            var user = await AuthManager.GetAuthorizedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            await _userAuthorizationService.AddUserRoleById(funcParams.UserId, userRole.RoleId, userRole.Attributes);

            return new NoContentResult();
        }
    }
}
