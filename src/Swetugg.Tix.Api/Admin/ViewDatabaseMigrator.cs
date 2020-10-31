using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Swetugg.Tix.Api.Admin
{
    public class ViewDatabaseMigrator
    {
        private readonly string _connectionString;
        private readonly IServiceProvider _serviceProvider;

        public ViewDatabaseMigrator(string connectionString)
        {
            _connectionString = connectionString;
            _serviceProvider = CreateServices();
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

        public void InitializeDatabase()
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = _serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        public void RemoveDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                TeardownDatabase(scope.ServiceProvider);
            }
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

        private static void TeardownDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(0);
        }

    }
}