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
    public class GetOrganizationInviteFuncParams
    {
        public string Token { get; set; } 
    }

    public class GetOrganizationInviteResponse
    {
        public UserInfo InvitedByUser { get; set; }
        public OrganizationInfo Organization { get; set; }
    }

    public class GetOrganizationInviteFunc : AuthorizedFunc<GetOrganizationInviteFuncParams>
    {
        private readonly IUserQueries _userQueries;
        private readonly IOrganizationQueries _organizationQueries;
        private readonly IJwtHelper _jwtHelper;

        public GetOrganizationInviteFunc(IUserQueries userQueries, IOrganizationQueries organizationQueries, IAuthManager authManager, IJwtHelper jwtHelper): base(authManager)
        {
            _userQueries = userQueries;
            _organizationQueries = organizationQueries;
            _jwtHelper = jwtHelper;
        }

        [FunctionName("GetOrganizationInvite")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organizations/invite")]
            HttpRequest req,
            ILogger log)
        {
            var token = req.Query["token"];
            return Process(req, log, new GetOrganizationInviteFuncParams { Token = token });
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, GetOrganizationInviteFuncParams funcParams)
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

            var invitedByUser = await _userQueries.GetUser(Guid.Parse(invitedByUserId));
            var organization = await _organizationQueries.GetOrganization(Guid.Parse(organizationId));

            
            return new OkObjectResult(new GetOrganizationInviteResponse { InvitedByUser = invitedByUser, Organization = organization });
        }
    }
}
