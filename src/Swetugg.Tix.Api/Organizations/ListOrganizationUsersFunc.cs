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

namespace Swetugg.Tix.Api.Organizations
{
    public class ListOrganizationUsersFuncParams
    {
        public Guid OrganizationId { get; set; }
    }

    public class ListOrganizationUsersFunc: AuthorizedFunc<ListOrganizationUsersFuncParams>
    {
        private readonly IUserQueries _userQueries;

        public ListOrganizationUsersFunc(IUserQueries userQueries, IAuthManager authManager) : base(authManager)
        {
            _userQueries = userQueries;
        }

        [FunctionName("ListOrganizationUsers")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organizations/{organizationId}/users")]
            HttpRequest req,
            ILogger log,
            Guid organizationId)
        {

            return Process(req, log, new ListOrganizationUsersFuncParams { OrganizationId = organizationId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, ListOrganizationUsersFuncParams funcParams)
        {
            var roles = await _userQueries.ListUsersByOrganization(funcParams.OrganizationId);

            return new OkObjectResult(roles);
        }
    }

}
