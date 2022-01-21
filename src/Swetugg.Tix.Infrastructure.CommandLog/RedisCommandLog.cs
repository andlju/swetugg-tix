using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Infrastructure.CommandLog
{
    public class RedisCommandLog : ICommandLog
    {
        ConnectionMultiplexer _redis;

        public RedisCommandLog(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public Task Store(Guid commandId, object command, string aggregateId = null)
        {
            var db = _redis.GetDatabase();
            var body = JsonSerializer.Serialize(command, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var vals = new Dictionary<RedisKey, RedisValue>
            {
                [$"command_body_{commandId}"] = body,
                [$"command_aggregate_{commandId}"] = aggregateId ?? Guid.Empty.ToString(),
                [$"command_status_{commandId}"] = CommandStatus.Created.ToString(),
                [$"command_revision_{commandId}"] = 0,
            };
            return db.StringSetAsync(vals.ToArray());
        }

        public Task Complete(Guid commandId, int? revision = null)
        {
            var db = _redis.GetDatabase();
            var vals = new Dictionary<RedisKey, RedisValue>
            {
                [$"command_revision_{commandId}"] = revision ?? 0,
                [$"command_status_{commandId}"] = CommandStatus.Completed.ToString(),
            };
            return db.StringSetAsync(vals.ToArray());
        }

        public Task Fail(Guid commandId, string code, string message)
        {
            var db = _redis.GetDatabase();
            var vals = new Dictionary<RedisKey, RedisValue>
            {
                [$"command_status_{commandId}"] = CommandStatus.Failed.ToString(),
                [$"command_code_{commandId}"] = code,
                [$"command_message_{commandId}"] = message,
            };
            return db.StringSetAsync(vals.ToArray());
        }

        public async Task<CommandLogItem> GetCommandLog(Guid commandId)
        {
            var db = _redis.GetDatabase();
            var res = await db.StringGetAsync(new RedisKey[] {
                $"command_body_{commandId}",
                $"command_aggregate_{commandId}",
                $"command_status_{commandId}",
                $"command_revision_{commandId}",
                $"command_code_{commandId}",
                $"command_message_{commandId}",
            });

            if (!res[0].HasValue)
                return null;

            var jsonBody = res[0].ToString();
            var aggregateId = res[1].ToString();
            Enum.TryParse<CommandStatus>(res[2], out var commandStatus);
            var revision = (int?)res[3];
            var code = res[4].HasValue ? res[4].ToString() : null;
            var message = res[5].HasValue ? res[5].ToString() : null;

            var item = new CommandLogItem
            {
                AggregateId = aggregateId,
                CommandId = commandId,
                JsonBody = jsonBody,
                Status = commandStatus.ToString(),
                Revision = revision
            };
            if (code != null)
            {
                item.Messages = new List<CommandLogMessage>
                {
                    new CommandLogMessage { Code = code, Message = message, Severity = CommandLogSeverity.Error }
                };
            }
            return item;
        }
    }
}
