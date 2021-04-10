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
    public class RoleAttributeView
    {
        public string Name { get; set; }
    }

    public class RoleView
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleAttributeView[] Attributes { get; set; }
    }
    public class ListRolesFunc: AuthorizedFunc<EmptyFuncParams>
    {
        private readonly IUserQueries _userQueries;

        public ListRolesFunc(IUserQueries userQueries, IAuthManager authManager) : base(authManager)
        {
            _userQueries = userQueries;
        }

        [FunctionName("ListRoles")]
        public Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "roles")]
            HttpRequest req,
            ILogger log)
        {
            return Process(req, log, null);
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, EmptyFuncParams funcParams)
        {
            var roles = await _userQueries.ListRoles();
            var roleViews = roles.Select(r => new RoleView
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Description = r.Description,
                // Get a list of unique attributes from the underlying permissions
                Attributes = r.Permissions.SelectMany(p => p.Attributes).GroupBy(p => p.Name).Select(a => new RoleAttributeView { Name = a.Key }).ToArray()
            });
            return new OkObjectResult(roleViews);
        }
    }
}
