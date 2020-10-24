using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore.Serialization.Json;
using Swetugg.Tix.Activity.Content;
using Swetugg.Tix.Activity.Domain;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Events.Admin;
using Swetugg.Tix.Activity.Funcs.Options;
using Swetugg.Tix.Activity.ViewBuilder;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Infrastructure.CommandLog;
using System.Data.SqlClient;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Activity.Funcs.Startup))]
namespace Swetugg.Tix.Activity.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ActivityOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<EventHubPublisher>();
            builder.Services.AddSingleton<ICommandLog>(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();
                var connectionString = options.Value.ViewsDbConnection;
                return new SqlDbCommandLog(connectionString, "ActivityLogs");
            });
            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();

                var eventStoreConnectionString = options.Value.ActivityEventsDbConnection;
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
            builder.Services.AddScoped<ActivityCommandListenerFunc, ActivityCommandListenerFunc>();
            builder.Services.AddScoped<EventHubViewBuilderFunc, EventHubViewBuilderFunc>();
            builder.Services.AddScoped<DatabaseManagementFunc, DatabaseManagementFunc>();

            builder.Services.AddSingleton<ViewBuilderHost>(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();
                var viewsConnectionString = options.Value.ViewsDbConnection;

                var host = ViewBuilderHost.Build(sp.GetService<ILoggerFactory>());

                host.RegisterViewBuilder(new ActivityOverviewBuilder(viewsConnectionString));
                host.RegisterViewBuilder(new TicketTypeBuilder(viewsConnectionString));

                return host;
            });

            builder.Services.AddSingleton<ViewDatabaseMigrator>(sp =>
            {
                var options = sp.GetService<IOptions<ActivityOptions>>();
                var viewsConnectionString = options.Value.ViewsDbConnection;
                var builder = new ViewDatabaseMigrator(viewsConnectionString);
                
                // Ensure that the database is properly initialized
                builder.InitializeDatabase();
                return builder;
            });

        }

    }
}