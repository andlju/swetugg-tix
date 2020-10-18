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
using Swetugg.Tix.Ticket.Domain;
using Swetugg.Tix.Ticket.Funcs.Options;
using System;
using System.Data.SqlClient;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Ticket.Funcs.Startup))]
namespace Swetugg.Tix.Ticket.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TicketOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<ServiceBusPublisher>();
            builder.Services.AddSingleton<EventHubPublisher>();
            builder.Services.AddSingleton<ICommandLog>(sp =>
            {
                var options = sp.GetService<IOptions<TicketOptions>>();
                var connectionString = options.Value.ViewsDbConnection;
                return new SqlDbCommandLog(connectionString, "TicketLogs");
            });

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<TicketOptions>>();

                var eventStoreConnectionString = options.Value.TicketEventsDbConnection;
                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, eventStoreConnectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                return DomainHost.Build(
                    eventStore,
                    sp.GetService<ServiceBusPublisher>(),
                    sp.GetService<EventHubPublisher>(),
                    sp.GetService<ILoggerFactory>(),
                    null,
                    sp.GetService<ICommandLog>());
            });

            builder.Services.AddScoped<TicketCommandListenerFunc, TicketCommandListenerFunc>();
        }
    }
}