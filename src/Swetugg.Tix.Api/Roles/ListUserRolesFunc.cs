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
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Roles
{
    public class ListUserRolesFuncParams
    {
        public Guid UserId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
    public class ListUserRolesFunc: AuthorizedFunc<ListUserRolesFuncParams>
    {
        private readonly IUserQueries _userQueries;

        public ListUserRolesFunc(IUserQueries userQueries, IAuthManager authManager) : base(authManager)
        {
            _userQueries = userQueries;
        }

        [FunctionName("ListUserRoles")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}/roles")]
            HttpRequest req,
            ILogger log,
            Guid userId)
        {
            var organizationIdString = req.Query["organizationId"];
            Guid.TryParse(organizationIdString, out Guid organizationId);

            return Process(req, log, new ListUserRolesFuncParams { UserId = userId, OrganizationId = organizationId != Guid.Empty ? organizationId : (Guid?) null });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, ListUserRolesFuncParams funcParams)
        {
            var roles = await _userQueries.ListUserRolesForUser(funcParams.UserId);

            return new OkObjectResult(roles);
        }
    }
}
