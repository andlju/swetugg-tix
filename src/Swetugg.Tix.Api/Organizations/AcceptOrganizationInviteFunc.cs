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

namespace Swetugg.Tix.Api.Organizations
{
    public class AcceptOrganizationInviteFuncParams
    {
        public string Token { get; set; } 
    }

    public class AcceptOrganizationInviteFunc : AuthorizedFunc<AcceptOrganizationInviteFuncParams>
    {

        private readonly IOrganizationCommands _organizationCommands;
        private readonly IJwtHelper _jwtHelper;

        public AcceptOrganizationInviteFunc(IOrganizationCommands organizationCommands, IAuthManager authManager, IJwtHelper jwtHelper): base(authManager)
        {
            _organizationCommands = organizationCommands;
            _jwtHelper = jwtHelper;
        }

        [FunctionName("AcceptOrganizationInvite")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "organizations/invite/accept")]
            HttpRequest req,
            ILogger log)
        {
            var token = req.Query["token"];
            return Process(req, log, new AcceptOrganizationInviteFuncParams { Token = token });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, AcceptOrganizationInviteFuncParams funcParams)
        {
            var user = await AuthManager.GetAuthenticatedUser();
            if (user.UserId == null)
                return new BadRequestResult();

            var claims = _jwtHelper.ValidateActionToken(funcParams.Token, "OrganizationInvite");
            
            if (claims == null || 
                !claims.TryGetValue("InvitedByUserId", out var invitedByUserId) ||
                !claims.TryGetValue("OrganizationId", out var organizationId))
            {
                return new BadRequestResult();
            }

            await _organizationCommands.LinkUserToOrganization(Guid.Parse(organizationId), user.UserId.Value);

            return new NoContentResult();
        }
    }
}
