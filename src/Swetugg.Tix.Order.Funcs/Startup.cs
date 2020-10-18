using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Infrastructure.CommandLog;
using Swetugg.Tix.Order.Domain;
using Swetugg.Tix.Order.Funcs.Options;
using System;
using System.Data.SqlClient;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Order.Funcs.Startup))]
namespace Swetugg.Tix.Order.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<OrderOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<EventHubPublisher>();
            builder.Services.AddSingleton<ICommandLog>(sp =>
            {
                var options = sp.GetService<IOptions<OrderOptions>>();
                var connectionString = options.Value.ViewsDbConnection;
                return new SqlDbCommandLog(connectionString, "OrderLogs");
            });

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<OrderOptions>>();

                var eventStoreConnectionString = options.Value.OrderEventsDbConnection;
                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, eventStoreConnectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                return DomainHost.Build(
                    eventStore,
                    sp.GetService<EventHubPublisher>(),
                    sp.GetService<ILoggerFactory>(),
                    null,
                    sp.GetService<ICommandLog>());
            });

            builder.Services.AddScoped<OrderCommandListenerFunc, OrderCommandListenerFunc>();
        }
    }
}