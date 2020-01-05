using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using Swetugg.Tix.Ticket.Commands;
using Swetugg.Tix.Ticket.Domain;
using Swetugg.Tix.Ticket.Funcs.Options;
using Swetugg.Tix.Infrastructure;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Ticket.Funcs.Startup))]
namespace Swetugg.Tix.Ticket.Funcs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TicketOptions>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            var eventStore = Wireup.Init().UsingInMemoryPersistence();
            builder.Services.AddSingleton<IEventPublisher, ServiceBusPublisher>();
            builder.Services.AddSingleton(sp => DomainHost.Build(eventStore, sp.GetService<IEventPublisher>(), sp.GetService<ILoggerFactory>(), null));

            builder.Services.AddScoped<TicketCommandListenerFunc, TicketCommandListenerFunc>();
        }
    }
}