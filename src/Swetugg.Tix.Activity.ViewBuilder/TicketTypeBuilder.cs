using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class TicketTypeBuilder : 
        IHandleEvent<TicketTypeAdded>,
        IHandleEvent<TicketTypeRemoved>,
        IHandleEvent<TicketTypeLimitIncreased>,
        IHandleEvent<TicketTypeLimitDecreased>,
        IHandleEvent<TicketTypeLimitRemoved>,
        IHandleEvent<SeatReserved>,
        IHandleEvent<SeatReturned>
    {
        private readonly string _connectionString;

        public TicketTypeBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Handle(TicketTypeAdded evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO ActivityViews.TicketType (ActivityId, TicketTypeId, Limit, Reserved) VALUES (@ActivityId, @TicketTypeId, @Limit, @Reserved)",
                    new TicketType()
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId,
                        Limit = null,
                        Reserved = 0,
                    });
            }
        }

        public async Task Handle(TicketTypeRemoved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "DELETE FROM ActivityViews.TicketType WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId)",
                    new
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId
                    });
            }
        }

        public async Task Handle(TicketTypeLimitIncreased evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.TicketType " +
                    "SET Limit = COALESCE(Limit, 0) + @Seats " + 
                    "WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId",
                    new 
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(TicketTypeLimitDecreased evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.TicketType " +
                    "SET Limit = Limit - @Seats " + 
                    "WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId",
                    new 
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId,
                        Seats = evt.Seats
                    });
            }
        }

        public async Task Handle(TicketTypeLimitRemoved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.TicketType " +
                    "SET Limit = NULL " + 
                    "WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId",
                    new 
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId,
                    });
            }
        }

        public async Task Handle(SeatReserved evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.TicketType " +
                    "SET Reserved = Reserved + 1 " + 
                    "WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId",
                    new 
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId
                    });
            }
        }

        public async Task Handle(SeatReturned evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE ActivityViews.TicketType " +
                    "SET Reserved = Reserved - 1 " + 
                    "WHERE ActivityId = @ActivityId AND TicketTypeId = @TicketTypeId",
                    new 
                    {
                        ActivityId = evt.AggregateId,
                        TicketTypeId = evt.TicketTypeId
                    });
            }
        }
    }
}