﻿using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Swetugg.Tix.Activity.Content;
using Swetugg.Tix.Activity.Content.Contract;
using Swetugg.Tix.Api.Activities;
using Swetugg.Tix.Api.Activities.Commands;
using Swetugg.Tix.Api.Admin;
using Swetugg.Tix.Api.Authorization;
using Swetugg.Tix.Api.Options;
using Swetugg.Tix.Api.Orders.Commands;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Infrastructure.CommandLog;
using Swetugg.Tix.Organization;
using Swetugg.Tix.User;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Api.Startup))]
namespace Swetugg.Tix.Api
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            IdentityModelEventSource.ShowPII = true;
            builder.Services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = Microsoft.Identity.Web.Constants.Bearer;
                sharedOptions.DefaultChallengeScheme = Microsoft.Identity.Web.Constants.Bearer;
            }).AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAdB2C"));
                // .EnableTokenAcquisitionToCallDownstreamApi()
                // .AddMicrosoftGraph(configuration.GetSection("GraphApi"))
                    //.AddInMemoryTokenCaches();
            // builder.Services.AddScoped<GraphApiClientDirect>();
            builder.Services.AddOptions<ApiOptions>()
                    .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddSingleton<IActivityCommandMessageSender, ActivityCommandMessageSender>();
            builder.Services.AddSingleton<IOrderCommandMessageSender, OrderCommandMessageSender>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IActivityContentCommands>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;

                return new SqlActivityContentCommands(viewsDbConnectionString);
            });

            builder.Services.AddSingleton<IUserCommands>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;
                return new UserCommands(viewsDbConnectionString);
            });
            builder.Services.AddSingleton<IUserQueries>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;
                return new UserQueries(viewsDbConnectionString);
            });
            builder.Services.AddSingleton<IOrganizationQueries>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;
                return new OrganizationQueries(viewsDbConnectionString);
            });
            builder.Services.AddSingleton<IOrganizationCommands>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var viewsDbConnectionString = options.Value.ViewsDbConnection;
                return new OrganizationCommands(viewsDbConnectionString);
            });
            builder.Services.AddSingleton<IJwtHelper>(sp => {
                var options = sp.GetService<IOptions<ApiOptions>>();
                return new JwtHelper(options.Value.ApiTokenSecurityKey);
            });
            builder.Services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
            builder.Services.AddScoped<IAuthManager, AuthManager>();
            builder.Services.AddSingleton<ICommandLog>(sp =>
            {
                var options = sp.GetService<IOptions<ApiOptions>>();
                var connectionString = options.Value.CommandLogCache;

                return new RedisCommandLog(connectionString);
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
                var builder = new ViewDatabaseMigrator(options.Value);

                // Ensure that the database is properly initialized
                builder.InitializeDatabase();
                return builder;
            });

        }
    }
}