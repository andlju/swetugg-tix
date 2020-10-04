using Dapper;
using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Events.CommandLog;
using Swetugg.Tix.Activity.Views;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class CommandLogBuilder :
        IHandleEvent<CommandBodyStoredLogEvent>,
        IHandleEvent<CommandCompletedLogEvent>,
        IHandleEvent<CommandFailedLogEvent>
    {
        private readonly string _connectionString;

        public CommandLogBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task EnsureCommandLog(SqlConnection conn, Guid commandId, Guid? activityId)
        {
            var hasLog = await conn.ExecuteScalarAsync<bool>("SELECT 1 FROM ActivityLogs.CommandLog WHERE CommandId = @CommandId", new { CommandId = commandId });
            if (hasLog)
                return;

            await conn.ExecuteAsync("INSERT INTO ActivityLogs.CommandLog (CommandId, ActivityId, Status, LastUpdated) VALUES (@CommandId, @ActivityId, @Status, SYSUTCDATETIME())", new {
                CommandId = commandId, 
                ActivityId = activityId,
                Status = CommandStatus.Created.ToString()
            });
        }

        public async Task Handle(CommandBodyStoredLogEvent evt)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, evt.CommandId, evt.ActivityId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " + 
                    "SET ActivityId = @ActivityId, " + 
                    "JsonBody = @JsonBody, " +
                    "LastUpdated = SYSUTCDATETIME() " + 
                    "WHERE CommandId = @CommandId",
                    evt);
                trans.Complete();
            }
        }

        public async Task Handle(CommandCompletedLogEvent evt)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, evt.CommandId, evt.ActivityId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " + 
                    "SET Status = @Status, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = evt.CommandId,
                        Status = CommandStatus.Completed.ToString()
                    });
                trans.Complete();
            }
        }

        public async Task Handle(CommandFailedLogEvent evt)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await EnsureCommandLog(conn, evt.CommandId, evt.ActivityId);
                await conn.ExecuteAsync(
                    "UPDATE ActivityLogs.CommandLog " +
                    "SET Status = @Status, " +
                    "LastUpdated = SYSUTCDATETIME() " +
                    "WHERE CommandId = @CommandId",
                    new
                    {
                        CommandId = evt.CommandId,
                        Status = CommandStatus.Failed.ToString()
                    });
                await conn.ExecuteAsync(
                    "INSERT INTO ActivityLogs.CommandLogMessages " +
                    "(CommandId, Severity, Code, Message, Timestamp) " +
                    "VALUES (@CommandId, @Severity, @Code, @Message, SYSUTCDATETIME())", 
                    new
                    {
                        CommandId = evt.CommandId,
                        Severity = CommandLogSeverity.Error,
                        Code = evt.Code,
                        Message = evt.Message
                    });
                trans.Complete();
            }
        }

    }
}