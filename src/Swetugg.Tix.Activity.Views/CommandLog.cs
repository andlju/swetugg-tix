using System;
using System.Collections.Generic;
using System.Text;

namespace Swetugg.Tix.Activity.Views
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
        public Guid? ActivityId { get; set; }
        public CommandStatus Status { get; set; }
        public string JsonBody { get; set; }
        public DateTime LastUpdated { get; set; }
        public CommandLogMessage[] Messages { get; set; }
    }

    public class CommandLogMessage
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public CommandLogSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
