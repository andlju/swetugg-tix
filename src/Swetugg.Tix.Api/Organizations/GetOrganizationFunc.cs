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
using Swetugg.Tix.User;
using Swetugg.Tix.User.Contract;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Organizations
{
    public class GetOrganizationFuncParams
    {
        public Guid OrganizationId { get; set; }
    }

    public class GetOrganizationFunc: AuthorizedFunc<GetOrganizationFuncParams>
    {
        private readonly IOrganizationQueries _orgQueries;

        public GetOrganizationFunc(IOrganizationQueries orgQueries, IAuthManager authManager) : base(authManager)
        {
            _orgQueries = orgQueries;
        }

        [FunctionName("GetOrganization")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organizations/{organizationId}")]
            HttpRequest req,
            ILogger log,
            Guid organizationId)
        {
            return Process(req, log, new GetOrganizationFuncParams { OrganizationId = organizationId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, GetOrganizationFuncParams funcParams)
        {
            var user = await AuthManager.GetAuthenticatedUser();

            if (user.UserId == null)
            {
                return new BadRequestResult();
            }
            var organization = await _orgQueries.GetOrganization(funcParams.OrganizationId);

            return new OkObjectResult(organization);
        }
    }
}
