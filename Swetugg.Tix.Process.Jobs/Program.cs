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
using Newtonsoft.Json;
using NEventStore;

namespace Swetugg.Tix.Process.Jobs
{

    public class LogSagaMessageDispatcher : ISagaMessageDispatcher
    {
        private readonly ILogger _logger;

        public LogSagaMessageDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        public void Dispatch(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            _logger.LogInformation(json);
        }
    }

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

            var eventStore = Wireup.Init().UsingInMemoryPersistence();

            serviceCollection.AddSingleton((sp) => new ProcessHost(eventStore, sp.GetService<LogSagaMessageDispatcher>()));

            serviceCollection.AddScoped<EventListener, EventListener>();

            // One more thing - tell azure where your azure connection strings are
            // Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("WebJobsDashboard"));
            // Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("WebJobsStorage"));
        }
    }
}
