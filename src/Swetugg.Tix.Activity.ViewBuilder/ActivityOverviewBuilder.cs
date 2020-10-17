using Dapper;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Events.Admin;
using Swetugg.Tix.Activity.Views;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class ActivityOverviewBuilder :
        IHandleEvent<ActivityCreated>,
        IHandleEvent<SeatsAdded>,
        IHandleEvent<SeatsRemoved>,
        IHandleEvent<SeatReserved>,
        IHandleEvent<SeatReturned>,
        IHandleEvent<TicketTypeAdded>,
        IHandleEvent<TicketTypeRemoved>,
        IHandleEvent<RebuildViewsRequested>,
        IHandleEvent<TicketTypeLimitIncreased>,
        IHandleEvent<TicketTypeLimitDecreased>,
        IHandleEvent<TicketTypeLimitRemoved>
    {
        private readonly string _connectionString;

        public ActivityOverviewBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task UpdateRevision(EventBase evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                    });
            }
        }

        public async Task Handle(RebuildViewsRequested evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "DELETE FROM ActivityViews.ActivityOverview WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                    });
            }
        }

        public async Task Handle(ActivityCreated evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO ActivityViews.ActivityOverview (ActivityId, Revision, TotalSeats, FreeSeats, TicketTypes) " +
                    "VALUES (@ActivityId, @Revision, @TotalSeats, @FreeSeats, @TicketTypes)",
                    new ActivityOverview()
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                        TotalSeats = 0,
                        FreeSeats = 0,
                        TicketTypes = 0
                    });
            }
        }

        public async Task Handle(SeatsAdded evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TotalSeats = TotalSeats + @Seats " +
                    ", FreeSeats = FreeSeats + @Seats " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(SeatsRemoved evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TotalSeats = TotalSeats - @Seats " +
                    ", FreeSeats = FreeSeats - @Seats " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(SeatReserved evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET FreeSeats = FreeSeats - 1 " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                    });
            }
        }

        public async Task Handle(SeatReturned evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET FreeSeats = FreeSeats + 1 " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                    });
            }
        }


        public async Task Handle(TicketTypeAdded evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TicketTypes = TicketTypes + 1 " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                    });
            }
        }

        public async Task Handle(TicketTypeRemoved evt, int revision)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TicketTypes = TicketTypes - 1 " +
                    ", Revision = @Revision " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Revision = revision,
                    });
            }
        }

        public Task Handle(TicketTypeLimitIncreased evt, int revision)
        {
            return UpdateRevision(evt, revision);
        }

        public Task Handle(TicketTypeLimitDecreased evt, int revision)
        {
            return UpdateRevision(evt, revision);
        }

        public Task Handle(TicketTypeLimitRemoved evt, int revision)
        {
            return UpdateRevision(evt, revision);
        }
    }
}