namespace Swetugg.Tix.Api.Options
{
    public class ApiOptions
    {
        public string AzureWebJobsStorage { get; set; }
        public string ActivityCommandsQueue { get; set; }
        public string OrderCommandsQueue { get; set; }
        public string TixServiceBus { get; set; }
        public string ViewsDbConnection { get; set; }
        public string CommandLogCache { get; set; }

    }
}