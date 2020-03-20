using System.Data.SqlClient;
using System.Threading.Tasks;
using Azure.Cosmos;
using Dapper;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public interface IHandleEvent<TEvent>
    {
        Task Handle(TEvent evt);
    }

    public class ActivityOverviewBuilder : IHandleEvent<ActivityCreated>, IHandleEvent<SeatsAdded>
    {
        private readonly string _databaseName;
        private readonly string _containerName;
        private CosmosClient _cosmosClient;

        public ActivityOverviewBuilder(
            string endpointUrl, 
            string authorizationKey, 
            string databaseName,
            string containerName)
        {
            _databaseName = databaseName;
            _containerName = containerName;
            _cosmosClient = new CosmosClient(endpointUrl, authorizationKey);
        }

        public async Task Handle(ActivityCreated evt)
        {
            var activityOverview = new ActivityOverview()
            {
                ActivityId = evt.AggregateId,
                Name = "Unnamed",
                TotalSeats = 0,
                FreeSeats = 0,
            };
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);

            await container.UpsertItemAsync(activityOverview);
        }

        public async Task Handle(SeatsAdded evt)
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            
            var activityOverviewResult = await container.ReadItemAsync<ActivityOverview>(evt.AggregateId.ToString(), new PartitionKey(evt.AggregateId.ToString()));

            var activityOverview = activityOverviewResult.Value;
            activityOverview.TotalSeats += evt.Seats;
            activityOverview.FreeSeats += evt.Seats;

            await container.ReplaceItemAsync(activityOverview, activityOverview.ActivityId.ToString(), new PartitionKey(activityOverview.ActivityId.ToString()));
        }
    }
}