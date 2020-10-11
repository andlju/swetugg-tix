using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ViewBuilderHost
    {
        private readonly string _connectionString;
        private IDictionary<Type, IList<Func<object, Task>>> _eventHandlers = new Dictionary<Type, IList<Func<object, Task>>>();

        public ViewBuilderHost(string connectionString)
        {
            _connectionString = connectionString;

            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(_connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migrations.InitialDatabase).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        public void RegisterHandler<TEvent>(IHandleEvent<TEvent> eventHandler)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handlerList))
            {
                _eventHandlers.Add(typeof(TEvent), handlerList = new List<Func<object, Task>>());
            };
            handlerList.Add(evt => eventHandler.Handle((TEvent)evt));
        }

        public static ViewBuilderHost Build(ILoggerFactory loggerFactory, string viewsConnectionString)
        {
            return new ViewBuilderHost(viewsConnectionString);
        }

        public async Task HandlePublishedEvent(PublishedEvent evt)
        {
            if (!_eventHandlers.TryGetValue(evt.Body.GetType(), out var handlers))
                return;

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                foreach (var handler in handlers)
                {
                    await handler(evt.Body);
                }
                trans.Complete();
            }
        }

    }
}