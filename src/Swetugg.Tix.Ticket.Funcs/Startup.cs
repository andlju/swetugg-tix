using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;
using Swetugg.Tix.Infrastructure;
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

            builder.Services.AddSingleton<IEventPublisher, ServiceBusPublisher>();
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = Environment.GetEnvironmentVariable("TicketEventsDbConnection");
                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, connectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                return DomainHost.Build(eventStore, sp.GetService<IEventPublisher>(),
                    sp.GetService<ILoggerFactory>(), null);
            });

            builder.Services.AddScoped<TicketCommandListenerFunc, TicketCommandListenerFunc>();
        }
    }
}