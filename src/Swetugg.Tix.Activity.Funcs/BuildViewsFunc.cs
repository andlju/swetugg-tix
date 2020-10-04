using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.ViewBuilder;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.Funcs
{
    public class BuildViewsFunc
    {
        private ViewBuilderHost _host;

        public BuildViewsFunc(ViewBuilderHost host)
        {
            _host = host;
        }

        [FunctionName("BuildViewsFunc")]
        public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            await _host.HandleCommits();
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
