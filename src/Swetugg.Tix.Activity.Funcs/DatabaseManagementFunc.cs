using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.ViewBuilder;

namespace Swetugg.Tix.Activity.Funcs
{
    public class DatabaseManagementFunc
    {
        private readonly ViewDatabaseMigrator _migrator;

        public DatabaseManagementFunc(ViewDatabaseMigrator migrator)
        {
            _migrator = migrator;
        }

        [FunctionName("DatabaseManagementFunc")]
        public void Run([TimerTrigger("0 0 0 3 * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Run Database management");
        }
    }
}
