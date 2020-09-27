﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // Queries
            builder.Services.AddScoped<GetActivityFunc, GetActivityFunc>();
            builder.Services.AddScoped<ListActivitiesFunc, ListActivitiesFunc>();
            builder.Services.AddScoped<ListTicketTypesFunc, ListTicketTypesFunc>();

            // Commands
            builder.Services.AddScoped<CreateActivityFunc, CreateActivityFunc>();
            builder.Services.AddScoped<AddSeatsFunc, AddSeatsFunc>();
            builder.Services.AddScoped<RemoveSeatsFunc, RemoveSeatsFunc>();
            builder.Services.AddScoped<AddTicketTypeFunc, AddTicketTypeFunc>();
            builder.Services.AddScoped<RemoveTicketTypeFunc, RemoveTicketTypeFunc>();
        }
    }
}