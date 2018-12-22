using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NEventStore;
using Swetugg.Tix.Activity.Domain;
using Swetugg.Tix.Activity.Jobs.Options;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Jobs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configRoot = BuildConfiguration(args);

            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                        .AddAzureStorage()
                        .AddServiceBus();
                })
                .ConfigureAppConfiguration(b =>
                {
                    b.AddConfiguration(configRoot.GetSection("AzureWebJobs"));
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();

                    // If this key exists in any config, use it to enable App Insights
                    string appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    if (!string.IsNullOrEmpty(appInsightsKey))
                    {
                        b.AddApplicationInsights(o => o.InstrumentationKey = appInsightsKey);
                    }
                })
                .ConfigureServices(services =>
                {
                    ConfigureServices(services, configRoot);
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }

        }
        private static IConfigurationRoot BuildConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Setup your container here, just like a asp.net core app

            // serviceCollection.Configure<MySettings>(configuration);
            serviceCollection.AddOptions();
            var dataSect = configuration.GetSection("Data");
            serviceCollection.Configure<StorageOptions>(configuration.GetSection("Data"));
            serviceCollection.Configure<MessagingOptions>(configuration.GetSection("Messaging"));

            var eventStore = Wireup.Init().UsingInMemoryPersistence();
            serviceCollection.AddSingleton<IEventPublisher, ServiceBusPublisher>();
            serviceCollection.AddSingleton(sp => DomainHost.Build(eventStore, sp.GetService<IEventPublisher>(), sp.GetService<ILoggerFactory>(), null));

            serviceCollection.AddScoped<CommandListener, CommandListener>();

            // One more thing - tell azure where your azure connection strings are
            // Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("WebJobsDashboard"));
            // Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("WebJobsStorage"));
        }
    }
}
