using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure.Views
{

    public abstract class TableStorageViewBuilder<TView, TEntity> : ViewBuilderBase<TView>
        where TEntity : TableEntity, IViewEntity<TView>, new()
        where TView: class, IView
    {

        private readonly CloudTable _table;

        public TableStorageViewBuilder(string storageConnectionString, string tableName, IEventApplier<TView> eventApplier): base(eventApplier)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration() { });

            // Create a table client for interacting with the table service 
            _table = tableClient.GetTableReference(tableName);

            _table.CreateIfNotExists();
        }

        protected abstract (string, string) GetKeys(string bucketId, string viewId);

        protected async override Task<TView> GetView(string bucketId, string viewId)
        {
            var (partitionKey, rowKey) = GetKeys(bucketId, viewId);
            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var result = await _table.ExecuteAsync(retrieveOperation);
            var entity = result.Result as TEntity;

            return entity?.ToView();
        }

        protected async override Task StoreView(TView oldView, TView newView)
        {
            var entity = new TEntity();
            entity.FromView(newView);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await _table.ExecuteAsync(insertOrMergeOperation);
        }

    }
}