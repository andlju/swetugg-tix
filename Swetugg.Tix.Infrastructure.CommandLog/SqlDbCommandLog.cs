using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace Swetugg.Tix.Infrastructure.CommandLog
{

    public class SqlDbCommandLog : ICommandLog
    {
        private readonly string _connectionString;

        public SqlDbCommandLog(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task EnsureCommandLog(SqlConnection conn, Guid commandId, string aggregateId)
        {
            var hasLog = await conn.ExecuteScalarAsync<bool>("SELECT 1 FROM ActivityLogs.CommandLog WHERE CommandId = @CommandId", new { CommandId = commandId });
            if (hasLog)
                return;

            await conn.ExecuteAsync("INSERT INTO ActivityLogs.CommandLog (CommandId, AggregateId, Status, LastUpdated) VALUES (@CommandId, @AggregateId, @Status, SYSUTCDATETIME())", new
            {
                CommandId = commandId,
                AggregateId = aggregateId,
                Status = CommandStatus.Created.ToString()
            });
        }

        public async Task Complete(Guid commandId)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, commandId, null);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " +
                    "SET Status = @Status, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = commandId,
                        Status = CommandStatus.Completed.ToString()
                    });
                trans.Complete();
            }
        }

        public async Task Fail(Guid commandId, string code, string message)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, commandId, null);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " +
                    "SET Status = @Status, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = commandId,
                        Status = CommandStatus.Failed.ToString()
                    });
                await conn.ExecuteAsync(
                    "INSERT INTO ActivityLogs.CommandLogMessages " +
                    "(CommandId, Severity, Code, Message, Timestamp) " +
                    "VALUES (@CommandId, @Severity, @Code, @Message, SYSUTCDATETIME())",
                    new
                    {
                        CommandId = commandId,
                        Severity = CommandLogSeverity.Error,
                        Code = code,
                        Message = message
                    });
                trans.Complete();
            }
        }

        public async Task Store(Guid commandId, object command, string aggregateId = null) 
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, commandId, aggregateId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " +
                    "SET AggregateId = @AggregateId, " +
                    "JsonBody = @JsonBody, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new {
                        CommandId = commandId,
                        AggregateId = aggregateId,
                        JsonBody = JsonSerializer.Serialize(command),
                    });
                trans.Complete();
            }
        }
    }
}
