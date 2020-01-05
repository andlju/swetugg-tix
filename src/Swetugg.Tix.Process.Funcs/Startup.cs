using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NEventStore;
using Newtonsoft.Json;
using Swetugg.Tix.Infrastructure;

[assembly: FunctionsStartup(typeof(Swetugg.Tix.Process.Funcs.Startup))]
namespace Swetugg.Tix.Process.Funcs
{
    public class LogSagaMessageDispatcher : ISagaMessageDispatcher
    {
        private readonly ILogger _logger;

        public LogSagaMessageDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        public void Dispatch(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            _logger.LogInformation(json);
        }
    }

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var eventStore = Wireup.Init().UsingInMemoryPersistence();

            builder.Services.AddSingleton((sp) => ProcessHost.Build(eventStore, sp.GetService<LogSagaMessageDispatcher>(), sp.GetService<ILoggerFactory>(), null));
            builder.Services.AddScoped<EventListenerFunc, EventListenerFunc>();
        }
    }
}