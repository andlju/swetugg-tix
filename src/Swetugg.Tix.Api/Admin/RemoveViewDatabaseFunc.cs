using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Admin
{
    public class RemoveViewDatabaseFunc
    {
        private readonly ViewDatabaseMigrator _migrator;

        public RemoveViewDatabaseFunc(ViewDatabaseMigrator migrator)
        {
            _migrator = migrator;
        }

        [FunctionName("RemoveViewDatabase")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "db-admin/views")]
            HttpRequest req,
            ILogger log)
        {
            _migrator.RemoveDatabase();

            return new OkObjectResult("All tables dropped");
        }
    }
}
