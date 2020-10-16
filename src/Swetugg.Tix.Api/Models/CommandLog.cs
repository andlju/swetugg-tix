using System;
using System.Collections.Generic;
using System.Text;

namespace Swetugg.Tix.Api.Models
{
    public enum CommandStatus 
    {
        Created,
        Completed,
        Failed
    }

    public enum CommandLogSeverity
    {
        Verbose = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4
    }

    public class CommandLog
    {
        public Guid CommandId { get; set; }
        public string AggregateId { get; set; }
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
