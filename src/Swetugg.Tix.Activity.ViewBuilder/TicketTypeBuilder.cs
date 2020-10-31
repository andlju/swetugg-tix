using Dapper;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class TicketTypeBuilder : ViewBuilderBase<TicketTypesView>
    {
        private readonly string _connectionString;

        public TicketTypeBuilder(string connectionString) : base(new TicketTypesEventApplier(), true)
        {
            _connectionString = connectionString;
        }

        protected override async Task<TicketTypesView> GetView(string viewId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var ticketTypes = (await conn.QueryAsync<TicketType>(
                    "SELECT ActivityId, TicketTypeId, Revision, Limit, Reserved " +
                    "FROM ActivityViews.TicketType " +
                    "WHERE ActivityId = @ActivityId ", new
                    {
                        ActivityId = viewId
                    })).ToList();
                return new TicketTypesView
                {
                    ActivityId = Guid.Parse(viewId),
                    Revision = ticketTypes.FirstOrDefault()?.Revision ?? 0,
                    TicketTypes = ticketTypes
                };
            }
        }

        protected override async Task StoreView(TicketTypesView oldView, TicketTypesView newView)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync("DELETE FROM ActivityViews.TicketType WHERE ActivityId = @ActivityId", new
                {
                    ActivityId = newView.ActivityId
                });
                foreach (var ticketType in newView.TicketTypes)
                {
                    await conn.ExecuteAsync(
                    "INSERT INTO ActivityViews.TicketType (ActivityId, TicketTypeId, Revision, Limit, Reserved) VALUES (@ActivityId, @TicketTypeId, @Revision, @Limit, @Reserved)",
                    new
                    {
                        ActivityId = newView.ActivityId,
                        TicketTypeId = ticketType.TicketTypeId,
                        Revision = newView.Revision,
                        Limit = ticketType.Limit,
                        Reserved = ticketType.Reserved
                    });
                }
                trans.Complete();
            }
        }
    }
}