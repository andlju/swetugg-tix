using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Swetugg.Tix.Activity.Jobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddUserSecrets();

            var config = configBuilder.Build();

            JobHostConfiguration jobHostConfig =
                new JobHostConfiguration(config["Data:AzureWebJobsStorage:ConnectionString"]);

            /*jobHostConfig.StorageConnectionString = config["Data:AzureWebJobsStorage:ConnectionString"];
            */
            jobHostConfig.UseServiceBus(new ServiceBusConfiguration()
            {
                ConnectionString = config["Data:AzureServiceBus:ConnectionString"]
            });
            JobHost host = new JobHost(jobHostConfig);
            host.RunAndBlock();
        }

        public static void DispatchCommand([ServiceBusTrigger("activitycommands")] string command)
        {
            Console.Out.WriteLine("Got command");
            Console.Out.WriteLine(command);
        }
    }
}
