using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using Swetugg.Tix.Activity.Domain;

namespace Swetugg.Tix.Activity.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var configRoot = BuildConfiguration();

            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configRoot);

            var configuration = new JobHostConfiguration(configRoot.GetSection("AzureWebJobs"));

            configuration.Queues.VisibilityTimeout = TimeSpan.FromSeconds(15);
            configuration.Queues.MaxDequeueCount = 3;
            configuration.LoggerFactory = new LoggerFactory().AddConsole();
            configuration.JobActivator = new CustomJobActivator(serviceCollection.BuildServiceProvider());
            // configuration.UseTimers();
            configuration.UseServiceBus();
            
            if (configuration.IsDevelopment)
            {
                configuration.UseDevelopmentSettings();
            }

            var host = new JobHost(configuration);
            Console.WriteLine($"StorageConn: {configuration.StorageConnectionString}"); 
            Console.WriteLine($"DashConn: {configuration.DashboardConnectionString}"); 
            host.RunAndBlock();
            
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Setup your container here, just like a asp.net core app

            // serviceCollection.Configure<MySettings>(configuration);

            var eventStore = Wireup.Init().UsingInMemoryPersistence();

            serviceCollection.AddSingleton(DomainHost.Build(eventStore));

            serviceCollection.AddScoped<CommandDispatcher, CommandDispatcher>();

            // One more thing - tell azure where your azure connection strings are
            // Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("WebJobsDashboard"));
            // Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("WebJobsStorage"));
        }
    }
}
