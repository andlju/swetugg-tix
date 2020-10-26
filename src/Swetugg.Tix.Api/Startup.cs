using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Activity.Content;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Api.Activities;
using Swetugg.Tix.Api.Activities.Commands;
using Swetugg.Tix.Api.Admin;
using Swetugg.Tix.Api.Options;
using Swetugg.Tix.Api.Orders.Commands;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Api.Startup))]
namespace Swetugg.Tix.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ApiOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<IActivityCommandMessageSender, ActivityCommandMessageSender>();
            builder.Services.AddSingleton<IOrderCommandMessageSender, OrderCommandMessageSender>();

            builder.Services.AddSingleton<IActivityContentCommands>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;

                return new SqlActivityContentCommands(viewsDbConnectionString);
            });

            builder.Services.AddSingleton<IActivityContentQuery>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;

                return new SqlActivityContentQuery(viewsDbConnectionString);
            });

            builder.Services.AddSingleton<ViewDatabaseMigrator>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsConnectionString = options.Value.ViewsDbConnection;
                var builder = new ViewDatabaseMigrator(viewsConnectionString);

                // Ensure that the database is properly initialized
                builder.InitializeDatabase();
                return builder;
            });

        }
    }
}