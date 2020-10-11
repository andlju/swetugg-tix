namespace Swetugg.Tix.Activity.Funcs.Options
{
    public class ActivityOptions
    {
        public string ActivityEventPublisherTopic { get; set; }
        public string AzureWebJobsStorage { get; set; }
        public string TixServiceBus { get; set; }
        public string ActivityEventsDbConnection { get; set; }
        public string ViewsDbConnection { get; set; }
        public string EventHubConnectionString { get; set; }
        public string ActivityEventHubName { get; set; }
        public string ActivityViewsConsumerGroup { get; set; }
    }
}