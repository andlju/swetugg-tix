namespace Swetugg.Tix.Order.Funcs.Options
{
    public class OrderOptions
    {
        public string AzureWebJobsStorage { get; set; }
        public string TixServiceBus { get; set; }
        public string OrderEventsDbConnection { get; set; }
        public string ViewsDbConnection { get; set; }
        public string EventHubConnectionString { get; set; }
        public string CommandLogCache { get; set; }
    }
}