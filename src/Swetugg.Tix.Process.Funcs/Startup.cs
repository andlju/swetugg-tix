using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;
using Polly;
using Polly.Registry;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Process.Funcs.Options;
using System;
using System.Data.SqlClient;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Process.Funcs.Startup))]
namespace Swetugg.Tix.Process.Funcs
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ProcessOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<ServiceBusMessageDispatcher>();

            builder.Services.AddSingleton<IPolicyRegistry<string>>(sp =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>();
                var retryPolicy = Policy.
                    Handle<Exception>().WaitAndRetryAsync(
                    5,
                    attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100),
                    onRetry: (ex, t) => loggerFactory.CreateLogger("RetryPolicy").LogError(ex, $"Retrying, attempt in {t.TotalMilliseconds}ms"));
                var registry = new PolicyRegistry();

                registry.Add("ProcessHost", retryPolicy);
                return registry;
            });

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<ProcessOptions>>();

                var eventStoreConnectionString = options.Value.ProcessEventsDbConnection;
                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, eventStoreConnectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                return ProcessHost.Build(
                    eventStore,
                    sp.GetService<ServiceBusMessageDispatcher>(),
                    sp.GetService<ILoggerFactory>(),
                    null,
                    sp.GetService<IPolicyRegistry<string>>());
            });

            builder.Services.AddScoped<ActivityEventListenerFunc, ActivityEventListenerFunc>();
        }
    }
}