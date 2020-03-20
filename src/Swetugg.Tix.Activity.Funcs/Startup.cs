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
                var connectionString = Environment.GetEnvironmentVariable("ActivityEventsDbConnection");
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
                var eventStoreConnectionString = Environment.GetEnvironmentVariable("ActivityEventsDbConnection");
                var endpointUrl = Environment.GetEnvironmentVariable("ViewsEndpointUrl");
                var authorizationKey = Environment.GetEnvironmentVariable("ViewsAuthorizationKey");
                var databaseName = Environment.GetEnvironmentVariable("ViewsDatabaseName");
                var viewsConnectionString = Environment.GetEnvironmentVariable("ViewsDbConnection");
                var viewsContainerName = Environment.GetEnvironmentVariable("ViewsContainerName");

                var sqlClientFactoryInstance = SqlClientFactory.Instance;

                var eventStore = Wireup.Init()
                    .UsingSqlPersistence(sqlClientFactoryInstance, eventStoreConnectionString)
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization();

                var host = ViewBuilderHost.Build(eventStore, sp.GetService<ILoggerFactory>(), viewsConnectionString);
                
                var activityOverviewBuilder = new ActivityOverviewBuilder(endpointUrl, authorizationKey, databaseName, viewsContainerName);
                host.RegisterHandler<ActivityCreated>(activityOverviewBuilder);
                host.RegisterHandler<SeatsAdded>(activityOverviewBuilder);
                return host;
            });
            builder.Services.AddScoped<BuildViewsFunc, BuildViewsFunc>();
        }
    }
}