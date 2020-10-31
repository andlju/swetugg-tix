using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace Swetugg.Tix.Infrastructure.CommandLog
{

    public class SqlDbCommandLog : ICommandLog
    {
        private readonly string _connectionString;
        private readonly string _schema;

        public SqlDbCommandLog(string connectionString, string schema)
        {
            _connectionString = connectionString;
            _schema = schema;
        }

        private async Task EnsureCommandLog(SqlConnection conn, Guid commandId, string aggregateId)
        {
            var hasLog = await conn.ExecuteScalarAsync<bool>(
                $"SELECT 1 FROM [{_schema}].CommandLog " +
                "WHERE CommandId = @CommandId", new { 
                    CommandId = commandId
                });
            if (hasLog)
                return;

            await conn.ExecuteAsync(
                $"INSERT INTO [{_schema}].CommandLog " +
                "(CommandId, AggregateId, Status, LastUpdated) " +
                "VALUES (@CommandId, @AggregateId, @Status, SYSUTCDATETIME())",
                new
                {
                    CommandId = commandId,
                    AggregateId = aggregateId,
                    Status = CommandStatus.Created.ToString()
                });
        }

        public async Task Complete(Guid commandId, int? revision)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, commandId, null);
                await conn.ExecuteAsync(
                    $"UPDATE [{_schema}].CommandLog " +
                    "SET Status = @Status, " +
                    "Revision = @Revision, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = commandId,
                        Revision = revision,
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
                    $"UPDATE [{_schema}].CommandLog " +
                    "SET Status = @Status, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = commandId,
                        Status = CommandStatus.Failed.ToString()
                    });
                await conn.ExecuteAsync(
                    $"INSERT INTO [{_schema}].CommandLogMessage " +
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
                    $"UPDATE [{_schema}].CommandLog " +
                    "SET AggregateId = @AggregateId, " +
                    "CommandType = @CommandType, " +
                    "JsonBody = @JsonBody, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = commandId,
                        AggregateId = aggregateId,
                        CommandType = command.GetType().Name,
                        JsonBody = JsonSerializer.Serialize(command, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    });
                trans.Complete();
            }
        }

        public Task<CommandLogItem> GetCommandLog(Guid commandId)
        {
            throw new NotImplementedException();
        }
    }
}
