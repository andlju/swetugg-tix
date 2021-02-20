namespace Swetugg.Tix.Api.Options
{
    public class ApiOptions
    {
        public string AzureWebJobsStorage { get; set; }
        public string TixServiceBus { get; set; }
        public string ViewsDbConnection { get; set; }
        public string CommandLogCache { get; set; }

        public string AzureAdClientId { get; set; }
        public string AzureAdTenantName { get; set; }
        public string AzureAdPolicyName { get; set; }

    }
}