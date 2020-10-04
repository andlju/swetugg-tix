using Dapper;
using Swetugg.Tix.Activity.Events;
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
        IHandleEvent<TicketTypeRemoved>
    {
        private readonly string _connectionString;

        public ActivityOverviewBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Handle(ActivityCreated evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO ActivityViews.ActivityOverview (ActivityId, TotalSeats, FreeSeats, TicketTypes) VALUES (@ActivityId, @TotalSeats, @FreeSeats, @TicketTypes)",
                    new ActivityOverview()
                    {
                        ActivityId = evt.AggregateId,
                        TotalSeats = 0,
                        FreeSeats = 0,
                        TicketTypes = 0
                    });
            }
        }

        public async Task Handle(SeatsAdded evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TotalSeats = TotalSeats + @Seats " +
                    ", FreeSeats = FreeSeats + @Seats " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(SeatsRemoved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TotalSeats = TotalSeats - @Seats " +
                    ", FreeSeats = FreeSeats - @Seats " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(SeatReserved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET FreeSeats = FreeSeats - 1 " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId
                    });
            }
        }

        public async Task Handle(SeatReturned evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET FreeSeats = FreeSeats + 1 " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId
                    });
            }
        }


        public async Task Handle(TicketTypeAdded evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TicketTypes = TicketTypes + 1 " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId
                    });
            }
        }

        public async Task Handle(TicketTypeRemoved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.ActivityOverview " +
                    "SET TicketTypes = TicketTypes - 1 " +
                    "WHERE ActivityId = @ActivityId",
                    new
                    {
                        ActivityId = evt.AggregateId
                    });
            }
        }

    }
}