using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Activity.Content;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Api.Commands;
using Swetugg.Tix.Api.Options;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Api.Startup))]
namespace Swetugg.Tix.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ApiOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<IMessageSender, ActivityCommandMessageSender>();

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

            // Queries
            builder.Services.AddScoped<GetActivityFunc, GetActivityFunc>();
            builder.Services.AddScoped<ListActivitiesFunc, ListActivitiesFunc>();
            builder.Services.AddScoped<ListTicketTypesFunc, ListTicketTypesFunc>();
            builder.Services.AddScoped<GetActivityCommandStatusFunc, GetActivityCommandStatusFunc>();

            // Commands
            builder.Services.AddScoped<CreateActivityFunc, CreateActivityFunc>();
            builder.Services.AddScoped<AddSeatsFunc, AddSeatsFunc>();
            builder.Services.AddScoped<RemoveSeatsFunc, RemoveSeatsFunc>();
            builder.Services.AddScoped<AddTicketTypeFunc, AddTicketTypeFunc>();
            builder.Services.AddScoped<RemoveTicketTypeFunc, RemoveTicketTypeFunc>();
        }
    }
}