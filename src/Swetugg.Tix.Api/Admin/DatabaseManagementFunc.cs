using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Swetugg.Tix.Api.Admin.Funcs
{
    public class DatabaseManagementFunc
    {
        private readonly ViewDatabaseMigrator _viewDbMigrator;

        public DatabaseManagementFunc(ViewDatabaseMigrator viewDbMigrator)
        {
            _viewDbMigrator = viewDbMigrator;
        }

        [FunctionName("DatabaseManagementFunc")]
        public void Run([TimerTrigger("0 0 0 3 * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Run Database management");
        }
    }
}
