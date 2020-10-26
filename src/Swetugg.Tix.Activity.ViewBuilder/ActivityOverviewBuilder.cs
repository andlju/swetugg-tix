using Dapper;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class ActivityOverviewBuilder : ViewBuilderBase<ActivityOverview>
    {
        private readonly string _connectionString;
        public ActivityOverviewBuilder(string connectionString) : base(new ActivityOverviewEventApplier(), true)
        {
            _connectionString = connectionString;
        }

        protected override async Task<ActivityOverview> GetView(string viewId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var view = await conn.QuerySingleOrDefaultAsync<ActivityOverview>(
                    "SELECT ActivityId, Revision, TotalSeats, FreeSeats, TicketTypes " +
                    "FROM ActivityViews.ActivityOverview " +
                    "WHERE ActivityId = @ActivityId", new
                    {
                        ActivityId = viewId
                    });

                if (view == null)
                {
                    await conn.ExecuteAsync(
                        "INSERT INTO ActivityViews.ActivityOverview (ActivityId, Revision, TotalSeats, FreeSeats, TicketTypes) " +
                        "VALUES (@ActivityId, @Revision, @TotalSeats, @FreeSeats, @TicketTypes)",
                        new ActivityOverview()
                        {
                            ActivityId = Guid.Parse(viewId),
                            Revision = 0,
                            TotalSeats = 0,
                            FreeSeats = 0,
                            TicketTypes = 0
                        });
                }
                return view;
            }
        }

        protected override async Task StoreView(ActivityOverview oldView, ActivityOverview newView)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET Revision = @Revision, " +
                    "TotalSeats = @TotalSeats, " +
                    "FreeSeats = @FreeSeats, " +
                    "TicketTypes = @TicketTypes " +
                    "WHERE ActivityId = @ActivityId",
                    newView);
            }
        }
    }
}