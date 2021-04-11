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

namespace Swetugg.Tix.Api.Organizations
{
    public class CreateOrganizationFunc : AuthorizedFunc<EmptyFuncParams>
    {

        private readonly IOrganizationCommands _organizationCommands;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public CreateOrganizationFunc(IOrganizationCommands organizationCommands, IAuthManager authManager, IUserAuthorizationService userAuthorizationService) : base(authManager)
        {
            _organizationCommands = organizationCommands;
            _userAuthorizationService = userAuthorizationService;
        }

        [FunctionName("CreateOrganization")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "organizations")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            var organizationInfo = await JsonSerializer.DeserializeAsync<OrganizationInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (organizationInfo.OrganizationId != Guid.Empty)
            {
                return new BadRequestResult();
            }

            if (string.IsNullOrEmpty(organizationInfo.Name))
                return new BadRequestResult();

            organizationInfo.OrganizationId = Guid.NewGuid();

            var user = await AuthManager.GetAuthorizedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _organizationCommands.CreateOrganization(organizationInfo, user.UserId.Value);
                // Automatically set the user who created the Organization to Admin
                await _userAuthorizationService.AddUserRoleByName(user.UserId.Value, "Admin", new[] {
                    new UserRoleAttribute { Name = "OrganizationId", Value = organizationInfo.OrganizationId.ToString() },
                    new UserRoleAttribute { Name = "ActivityId", Value = "*" } });
                trans.Complete();
            }

            return new NoContentResult();
        }
    }
}
