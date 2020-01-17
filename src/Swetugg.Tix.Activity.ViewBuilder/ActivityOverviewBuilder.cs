using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public interface IHandleEvent<TEvent>
    {
        Task Handle(TEvent evt);
    }

    public class ActivityOverviewBuilder : IHandleEvent<ActivityCreated>, IHandleEvent<SeatsAdded>
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
                    "INSERT INTO ActivityOverview (ActivityId, Name, TotalSeats, FreeSeats) VALUES (@ActivityId, @Name, @TotalSeats, @FreeSeats)",
                    new ActivityOverview()
                    {
                        ActivityId = evt.AggregateId,
                        Name = "Unnamed",
                        TotalSeats = 0,
                        FreeSeats = 0,
                    });
            }
        }

        public async Task Handle(SeatsAdded evt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // Update number of seats
            }
        }
    }
}