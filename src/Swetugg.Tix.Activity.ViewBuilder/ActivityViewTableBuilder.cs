using Microsoft.Azure.Cosmos.Table;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ActivityViewTableBuilder : ViewBuilderBase<ActivityOverview>
    {
        class ActivityViewEntity : TableEntity
        {
            public ActivityViewEntity()
            {

            }

            public Guid ActivityId { get; set; }
            public int Revision { get; set; }
            public string Name { get; set; }
            public int TotalSeats { get; set; }
            public int FreeSeats { get; set; }
            public int TicketTypes { get; set; }

            public ActivityViewEntity(ActivityOverview view)
            {
                var key = view.ActivityId.ToString();
                PartitionKey = key;
                RowKey = key;
                ActivityId = view.ActivityId;
                Revision = view.Revision;
                Name = view.Name;
                TotalSeats = view.TotalSeats;
                FreeSeats = view.FreeSeats;
                TicketTypes = view.TicketTypes;
            }

            public ActivityOverview ToView()
            {
                return new ActivityOverview
                {
                    ActivityId = this.ActivityId,
                    Revision = this.Revision,
                    Name = this.Name,
                    TotalSeats = this.TotalSeats,
                    FreeSeats = this.FreeSeats,
                    TicketTypes = this.TicketTypes,
                };
            }
        }

        private const string _tableName = "activityview";
        private readonly CloudTable _table;

        public ActivityViewTableBuilder(string storageConnectionString) : base(new ActivityOverviewEventApplier())
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration() { });

            // Create a table client for interacting with the table service 
            _table = tableClient.GetTableReference(_tableName);
        }

        
        protected async override Task<ActivityOverview> GetView(string viewId)
        {
            var retrieveOperation = TableOperation.Retrieve<ActivityViewEntity>(viewId, viewId);
            var result = await _table.ExecuteAsync(retrieveOperation);
            var entity = result.Result as ActivityViewEntity;

            return entity?.ToView();
        }

        protected async override Task StoreView(ActivityOverview oldView, ActivityOverview newView)
        {
            var entity = new ActivityViewEntity(newView);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await _table.ExecuteAsync(insertOrMergeOperation);
        }
    }
}