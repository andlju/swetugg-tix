using Swetugg.Tix.Activity.Content.Contract;
using System;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.Content
{
    public class SqlActivityContentCommands : IActivityContentCommands
    {
        private readonly string _connectionString;

        public SqlActivityContentCommands(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task EnsureActivity(SqlConnection conn, Guid activityId)
        {
            var hasActivity = await conn.ExecuteScalarAsync<bool>(
                "SELECT 1 FROM ActivityContent.Activity " +
                "WHERE ActivityId = @ActivityId", new { ActivityId = activityId });
            if (hasActivity)
                return;

            await conn.ExecuteAsync(
                "INSERT INTO ActivityContent.Activity (ActivityId, Name, LastUpdated) " +
                "VALUES (@ActivityId, @Name, SYSUTCDATETIME())",
                new
                {
                    ActivityId = activityId,
                    Name = "Unnamed",
                });
        }

        public async Task StoreActivityContent(ActivityContent content)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureActivity(conn, content.ActivityId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityContent.Activity " +
                    "SET Name              = @Name " +
                    ",   LastUpdated       = SYSUTCDATETIME() " +
                    "WHERE ActivityId = @ActivityId", content);
                trans.Complete();
            }
        }

        private async Task EnsureTicketType(SqlConnection conn, Guid activityId, Guid ticketTypeId)
        {
            var hasActivity = await conn.ExecuteScalarAsync<bool>(
                "SELECT 1 FROM ActivityContent.TicketType " +
                "WHERE ActivityId = @ActivityId " +
                "AND TicketTypeId = @TicketTypeId", new { ActivityId = activityId, TicketTypeId = ticketTypeId });
            if (hasActivity)
                return;

            await conn.ExecuteAsync(
                "INSERT INTO ActivityContent.TicketType (ActivityId, TicketTypeId, Name, LastUpdated) " +
                "VALUES (@ActivityId, @TicketTypeId, @Name, SYSUTCDATETIME())",
                new
                {
                    ActivityId = activityId,
                    TicketTypeId = ticketTypeId,
                    Name = "Unnamed",
                });
        }

        public async Task StoreTicketTypeContent(TicketTypeContent content)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureTicketType(conn, content.ActivityId, content.TicketTypeId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityContent.TicketType " +
                    "SET Name              = @Name " +
                    ",   LastUpdated       = SYSUTCDATETIME() " +
                    "WHERE ActivityId = @ActivityId " +
                    "AND TicketTypeId = @TicketTypeId", content);
                trans.Complete();
            }
        }
    }
}
