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

namespace Swetugg.Tix.Api.Activities
{
    public class CreateOrganizationFunc : AuthorizedFunc<EmptyFuncParams>
    {

        private readonly IOrganizationCommands _organizationCommands;

        public CreateOrganizationFunc(IOptions<ApiOptions> options, IOrganizationCommands organizationCommands, IAuthManager authManager): base(authManager)
        {
            _organizationCommands = organizationCommands;
        }

        [FunctionName("CreateOrganization")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "organization")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            var organizationInfo = await JsonSerializer.DeserializeAsync<OrganizationInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (organizationInfo.OrganizationId != null)
            {
                return new BadRequestResult();
            }

            var user = await AuthManager.GetAuthenticatedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            await _organizationCommands.CreateOrganization(organizationInfo, user.UserId.Value);

            return new NoContentResult();
        }
    }
}
