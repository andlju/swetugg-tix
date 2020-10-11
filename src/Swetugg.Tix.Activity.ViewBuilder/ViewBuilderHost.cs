using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using NEventStore.Persistence;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ViewBuilderHost
    {
        private IPersistStreams _store;
        private readonly string _connectionString;
        private IDictionary<Type, IList<Func<object, Task>>> _eventHandlers = new Dictionary<Type, IList<Func<object, Task>>>();

        public ViewBuilderHost(IPersistStreams store, string connectionString)
        {
            _store = store;
            _connectionString = connectionString;

            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                var hasCheckpoint = conn.ExecuteScalar<int>("SELECT COUNT(Name) FROM [Checkpoint] WHERE Name=@name", new { name = "ViewBuilder" });
                if (hasCheckpoint <= 0)
                {
                    conn.Execute("INSERT INTO [Checkpoint] (Name, LastCheckpoint) VALUES(@Name,0)", new { name = "ViewBuilder" });
                }
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

        public static ViewBuilderHost Build(Wireup eventStoreWireup, ILoggerFactory loggerFactory, string viewsConnectionString)
        {
            var eventStore =
                eventStoreWireup
                    .Build();

            return new ViewBuilderHost(eventStore.Advanced, viewsConnectionString);
        }

        public async Task HandleCommits()
        {
            long checkpoint = 0;
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                checkpoint = await conn.ExecuteScalarAsync<long>("SELECT LastCheckpoint FROM [Checkpoint] WHERE Name=@name", new { name = "ViewBuilder" });

                var commits = _store.GetFrom(checkpoint);
                foreach (var commit in commits)
                {
                    checkpoint = commit.CheckpointToken;
                    foreach (var evt in commit.Events)
                    {
                        if (!_eventHandlers.TryGetValue(evt.Body.GetType(), out var handlers))
                            continue;

                        foreach (var handler in handlers)
                        {
                            await handler(evt.Body);
                        }
                    }
                }

                var result = await conn.ExecuteAsync("UPDATE [Checkpoint] SET LastCheckpoint = @checkpoint WHERE Name=@name", new { name = "ViewBuilder", checkpoint });
                trans.Complete();
            }
        }

    }
}