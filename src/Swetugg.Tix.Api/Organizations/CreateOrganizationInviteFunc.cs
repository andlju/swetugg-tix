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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Organizations
{
    public class CreateOrganizationInviteFuncParams
    {
        public Guid OrganizationId { get; set; }
    }

    public class CreateOrganizationInviteFunc : AuthorizedFunc<CreateOrganizationInviteFuncParams>
    {
        private readonly IJwtHelper _jwtHelper;

        public CreateOrganizationInviteFunc(IAuthManager authManager, IJwtHelper jwtHelper) : base(authManager)
        {
            _jwtHelper = jwtHelper;
        }

        [FunctionName("CreateOrganizationInvite")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "organizations/{organizationId}/invite")]
            HttpRequest req,
            ILogger log,
            Guid organizationId)
        {
            return Process(req, log, new CreateOrganizationInviteFuncParams { OrganizationId = organizationId });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, CreateOrganizationInviteFuncParams funcParams)
        {
            var user = await AuthManager.GetAuthenticatedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            if (funcParams.OrganizationId == Guid.Empty)
                return new BadRequestResult();

            var token = _jwtHelper.GenerateActionJwtToken("OrganizationInvite", DateTime.UtcNow.AddDays(2), new Dictionary<string, string>
            {
                ["InvitedByUserId"] = user.UserId.Value.ToString(),
                ["OrganizationId"] = funcParams.OrganizationId.ToString(),
            });

            return new OkObjectResult(token);
        }
    }
}
