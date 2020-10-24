using Microsoft.Extensions.Logging;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ViewBuilderHost
    {
        private IList<IViewBuilder> _viewBuilders = new List<IViewBuilder>();

        public void RegisterViewBuilder(IViewBuilder viewBuidler)
        {
            _viewBuilders.Add(viewBuidler);
        }

        public static ViewBuilderHost Build(ILoggerFactory loggerFactory)
        {
            return new ViewBuilderHost();
        }

        public async Task HandlePublishedEvents(IEnumerable<PublishedEvent> evts)
        {
            foreach(var builder in _viewBuilders)
            {
                await builder.HandleEvents(evts);
            }
        }

    }
}