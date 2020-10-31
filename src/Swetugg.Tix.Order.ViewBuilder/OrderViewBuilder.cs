using Dapper;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

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
                var lookup = new Dictionary<Guid, OrderView>();
                var view = (await conn.QueryAsync<OrderView, OrderTicket, OrderView>(
                    "SELECT ov.OrderId, ov.ActivityId, ov.Revision, ot.TicketId, ot.OrderId, ot.TicketTypeId, ot.TicketReference " +
                    "FROM OrderViews.OrderView ov LEFT JOIN OrderViews.OrderTicket ot ON ov.OrderId = ot.OrderId " +
                    "WHERE ov.OrderId = @OrderId", (ov, ot) => {
                        if (!lookup.TryGetValue(ov.OrderId, out var orderView))
                            lookup.Add(ov.OrderId, orderView = ov);
                        if (orderView.Tickets == null)
                            orderView.Tickets = new List<OrderTicket>();
                        if (ot != null)
                        {
                            ot.ActivityId = orderView.ActivityId.GetValueOrDefault();
                            orderView.Tickets.Add(ot);
                        }
                        return orderView;
                    }, new
                    {
                        OrderId = viewId,
                    }, splitOn: "TicketId")).FirstOrDefault();

                if (view != null)
                    return view;

                view = new OrderView()
                {
                    OrderId = Guid.Parse(viewId),
                    ActivityId = null,
                    Revision = 0,
                    Tickets = new List<OrderTicket>()
                };

                await conn.ExecuteAsync(
                    "INSERT INTO OrderViews.OrderView (OrderId, ActivityId, Revision) " +
                    "VALUES (@OrderId, @ActivityId, @Revision)", view);

                return view;
            }
        }

        protected override async Task StoreView(OrderView oldView, OrderView newView)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE OrderViews.OrderView " +
                    "SET ActivityId = @ActivityId, " +
                    "Revision = @Revision " +
                    "WHERE OrderId = @OrderId",
                    newView);

                await conn.ExecuteAsync(
                    "DELETE FROM OrderViews.OrderTicket WHERE OrderId = @OrderId", new { newView.OrderId });

                foreach(var ticket in newView.Tickets)
                {
                    await conn.ExecuteAsync(
                        "INSERT INTO OrderViews.OrderTicket " +
                        "(TicketId, OrderId, ActivityId, TicketTypeId, TicketReference) " +
                        "VALUES (@TicketId, @OrderId, @ActivityId, @TicketTypeId, @TicketReference)",
                        ticket);
                }
                trans.Complete();
            }
        }
    }
}
