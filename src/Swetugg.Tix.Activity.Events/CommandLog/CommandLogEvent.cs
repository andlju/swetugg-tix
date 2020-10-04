using System;

namespace Swetugg.Tix.Activity.Events.CommandLog
{
    public abstract class CommandLogEvent
    {
        public Guid CommandId { get; set; }
        public Guid? ActivityId { get; set; }
    }
}