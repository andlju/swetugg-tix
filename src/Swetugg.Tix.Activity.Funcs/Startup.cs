using System;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Funcs.Options;
using Swetugg.Tix.Activity.ViewBuilder;
using Swetugg.Tix.Infrastructure;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Activity.Funcs.Startup))]
namespace Swetugg.Tix.Activity.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ActivityOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<IEventPublisher, ServiceBusPublisher>();

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();
                
                var connectionString = options.Value.ActivityEventsDbConnection;
                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, connectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                return DomainHost.Build(eventStore, sp.GetService<IEventPublisher>(),
                        sp.GetService<ILoggerFactory>(), null);
            });
            builder.Services.AddScoped<ActivityCommandListenerFunc, ActivityCommandListenerFunc>();

            builder.Services.AddSingleton<ViewBuilderHost>(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();
                var eventStoreConnectionString = options.Value.ActivityEventsDbConnection;
                var viewsConnectionString = options.Value.ViewsDbConnection;

                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, eventStoreConnectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                var host = ViewBuilderHost.Build(eventStore, sp.GetService<ILoggerFactory>(), viewsConnectionString);
                
                // Register ActivityOverviewBuilder
                host.RegisterHandler<ActivityCreated>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<SeatsAdded>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<SeatsRemoved>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<SeatReserved>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<SeatReturned>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeAdded>(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeRemoved>(new ActivityOverviewBuilder(viewsConnectionString));

                // Register TicketTypeBuilder
                host.RegisterHandler<TicketTypeAdded>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeRemoved>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeLimitIncreased>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeLimitDecreased>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<TicketTypeLimitRemoved>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<SeatReserved>(new TicketTypeBuilder(viewsConnectionString));
                host.RegisterHandler<SeatReturned>(new TicketTypeBuilder(viewsConnectionString));

                return host;
            });
            builder.Services.AddScoped<BuildViewsFunc, BuildViewsFunc>();
        }
    }
}