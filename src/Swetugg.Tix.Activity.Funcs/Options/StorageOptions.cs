namespace Swetugg.Tix.Activity.Funcs.Options
{
    public class ConnectionStringOption
    {
        public string ConnectionString { get; set; }
    }

    public class StorageOptions
    {
        public ConnectionStringOption AzureWebJobsStorage { get; set; }
        public ConnectionStringOption AzureServiceBus { get; set; }
    }
}