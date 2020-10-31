using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface ICommandLog
    {
        Task Store(Guid commandId, object command, string aggregateId = null);
        Task Complete(Guid commandId, int? revision = null);
        Task Fail(Guid commandId, string code, string message);
        Task<CommandLogItem> GetCommandLog(Guid commandId);
    }

    public class CommandLogItem
    {
        public Guid CommandId { get; set; }
        public string AggregateId { get; set; }
        public int? Revision { get; set; }
        public string CommandType { get; set; }
        public string Status { get; set; }
        public string JsonBody { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<CommandLogMessage> Messages { get; set; }
    }

    public class CommandLogMessage
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public CommandLogSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
