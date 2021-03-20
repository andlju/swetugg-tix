using Microsoft.Azure.Cosmos.Table;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Swetugg.Tix.Api
{
    public static class CloudTableExtensions
    {
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {
            var runningQuery = new TableQuery<T>()
            {
                FilterString = query.FilterString,
                SelectColumns = query.SelectColumns
            };

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                runningQuery.TakeCount = query.TakeCount - items.Count;

                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(runningQuery, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null) onProgress(items);

            } while (token != null && !ct.IsCancellationRequested && (query.TakeCount == null || items.Count < query.TakeCount.Value));

            return items;
        }
    }

    public class TableStorageViewReader
    {
        private readonly CloudTable _table;

        public TableStorageViewReader(string storageConnectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Create a table client for interacting with the table service 
            _table = tableClient.GetTableReference(tableName);
        }

        public async Task<TView> GetEntity<TEntity, TView>(string partitionKey, string rowKey)
            where TEntity : TableEntity, IViewEntity<TView>, new()
            where TView : class, IView
        {
            var retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var result = await _table.ExecuteAsync(retrieveOperation);
            var entity = result.Result as TEntity;
            return entity?.ToView();
        }

        public async Task<IEnumerable<TView>> ListAllEntities<TEntity, TView>()
            where TEntity : TableEntity, IViewEntity<TView>, new()
            where TView : class, IView
        {
            var query = new TableQuery<TEntity>();
            var result = await _table.ExecuteQueryAsync(query);

            return result.Select(e => e?.ToView());
        }

        public async Task<IEnumerable<TView>> ListEntitiesForPartition<TEntity, TView>(string partitionKey)
            where TEntity : TableEntity, IViewEntity<TView>, new()
            where TView : class, IView
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<TEntity>().Where(filter);
            var result = await _table.ExecuteQueryAsync(query);

            return result.Select(e => e?.ToView());
        }

    }
}
