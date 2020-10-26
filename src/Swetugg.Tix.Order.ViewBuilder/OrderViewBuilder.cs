using Dapper;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Views;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.ViewBuilder
{
    public class OrderViewBuilder : ViewBuilderBase<OrderView>
    {
        private readonly string _connectionString;

        public OrderViewBuilder(string connectionString) : base(new OrderViewEventApplier(), true)
        {
            _connectionString = connectionString;
        }

        protected override async Task<OrderView> GetView(string viewId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var view = await conn.QuerySingleOrDefaultAsync<OrderView>(
                    "SELECT OrderId, Revision " +
                    "FROM OrderViews.OrderView " +
                    "WHERE OrderId = @OrderId", new
                    {
                        OrderId = viewId
                    });

                if (view == null)
                {
                    await conn.ExecuteAsync(
                        "INSERT INTO OrderViews.OrderView (OrderId, Revision) " +
                        "VALUES (@OrderId, @Revision)",
                        new OrderView()
                        {
                            OrderId = Guid.Parse(viewId),
                            Revision = 0,
                        });
                }
                return view;
            }
        }

        protected override async Task StoreView(OrderView oldView, OrderView newView)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE OrderViews.OrderView " +
                    "SET Revision = @Revision " +
                    "WHERE OrderId = @OrderId",
                    newView);
            }
        }
    }
}
