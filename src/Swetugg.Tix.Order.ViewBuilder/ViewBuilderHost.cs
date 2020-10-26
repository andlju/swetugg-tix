using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.ViewBuilder
{
    public class ViewBuilderHost
    {
        private readonly IList<IViewBuilder> _viewBuilders = new List<IViewBuilder>();
        private readonly ILogger _logger;
        private readonly IPolicyRegistry<string> _policyRegistry;

        public ViewBuilderHost(ILoggerFactory loggerFactory, IPolicyRegistry<string> policyRegistry)
        {
            _logger = loggerFactory.CreateLogger<ViewBuilderHost>();
            _policyRegistry = policyRegistry;
        }

        public void RegisterViewBuilder(IViewBuilder viewBuidler)
        {
            _viewBuilders.Add(viewBuidler);
        }

        public static ViewBuilderHost Build(ILoggerFactory loggerFactory, IPolicyRegistry<string> policyRegistry)
        {
            return new ViewBuilderHost(loggerFactory, policyRegistry);
        }

        public async Task HandlePublishedEvents(IEnumerable<PublishedEvent> evts)
        {
            await Task.WhenAll(_viewBuilders.Select(vb =>
                _policyRegistry.Get<IAsyncPolicy>(vb.GetType().Name).ExecuteAsync(() => vb.HandleEvents(evts))
            ));
        }

    }
}