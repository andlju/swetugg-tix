using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swetugg.Tix.Infrastructure;
using System.Collections;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process.Funcs
{

    public class LogSagaMessageDispatcher : ISagaMessageDispatcher
    {
        private readonly ILogger _logger;

        public LogSagaMessageDispatcher(ILogger<LogSagaMessageDispatcher> logger)
        {
            _logger = logger;
        }

        public Task Dispatch(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            _logger.LogInformation(json);
            return Task.FromResult(0);
        }
    }
}