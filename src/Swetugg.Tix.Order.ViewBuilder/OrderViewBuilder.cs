using Swetugg.Tix.Infrastructure.Views;
using Swetugg.Tix.Order.Views;
using Swetugg.Tix.Order.Views.TableStorage;

namespace Swetugg.Tix.Order.ViewBuilder
{
    public class OrderViewTableBuilder : TableStorageViewBuilder<OrderView, OrderViewEntity>
    {
        private const string _tableName = "orderview";

        public OrderViewTableBuilder(string storageConnectionString) : base(storageConnectionString, _tableName, new OrderViewEventApplier())
        {
        }

        protected override (string, string) GetKeys(string bucketId, string viewId)
        {
            return (viewId, viewId);
        }
    }

}
