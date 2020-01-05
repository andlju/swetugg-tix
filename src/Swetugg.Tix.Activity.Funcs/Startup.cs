using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain;
using Swetugg.Tix.Activity.Funcs.Options;
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

            var eventStore = Wireup.Init().UsingInMemoryPersistence();
            builder.Services.AddSingleton<IEventPublisher, ServiceBusPublisher>();
            builder.Services.AddSingleton(sp => DomainHost.Build(eventStore, sp.GetService<IEventPublisher>(), sp.GetService<ILoggerFactory>(), null));

            builder.Services.AddScoped<ActivityCommandListenerFunc, ActivityCommandListenerFunc>();
        }
    }
}