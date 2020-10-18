namespace Swetugg.Tix.Ticket.Funcs.Options
{
    public class TicketOptions
    {
        public string TicketEventPublisherTopic { get; set; }
        public string AzureWebJobsStorage { get; set; }
        public string TixServiceBus { get; set; }
        public string TicketEventsDbConnection { get; set; }
        public string ViewsDbConnection { get; set; }
        public string EventHubConnectionString { get; set; }
        public string TicketEventHubName { get; set; }
        public string TicketViewsConsumerGroup { get; set; }
    }
}