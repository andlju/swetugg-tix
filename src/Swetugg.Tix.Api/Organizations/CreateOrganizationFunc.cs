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
    public class CreateOrganizationFunc
    {
        private static string[] acceptedScopes = new[] { "access_as_user", "access_as_admin" };

        private readonly IOrganizationCommands _organizationCommands;
        private readonly IAuthManager _authManager;

        public CreateOrganizationFunc(IOptions<ApiOptions> options, IOrganizationCommands organizationCommands, IAuthManager authManager)
        {
            _organizationCommands = organizationCommands;
            _authManager = authManager;
        }

        [FunctionName("CreateOrganization")]
        // [RequiredScope("access_as_user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "organization")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            var identity = req.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var subject = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var issuer = identity.FindFirst("iss").Value;

            var organizationInfo = await JsonSerializer.DeserializeAsync<OrganizationInfo>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (organizationInfo.OrganizationId != null)
            {
                return new BadRequestResult();
            }

            await _organizationCommands.CreateOrganization(organizationInfo, Guid.Empty);

            return new NoContentResult();
        }
    }
}
