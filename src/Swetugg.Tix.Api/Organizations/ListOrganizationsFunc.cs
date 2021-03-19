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
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities
{
    public class ListOrganizationsFunc: AuthorizedFunc<EmptyFuncParams>
    {
        private readonly IOrganizationQueries _orgQueries;

        public ListOrganizationsFunc(IOrganizationQueries orgQueries, IAuthManager authManager) : base(authManager)
        {
            _orgQueries = orgQueries;
        }

        [FunctionName("ListOrganizations")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organizations")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            var user = await AuthManager.GetAuthenticatedUser();

            if (user.UserId == null)
            {
                return new BadRequestResult();
            }
            var organizations = await _orgQueries.ListOrganizations(user.UserId.Value);

            return new OkObjectResult(organizations);
        }
    }
}
