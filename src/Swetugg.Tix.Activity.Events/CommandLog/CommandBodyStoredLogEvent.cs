namespace Swetugg.Tix.Activity.Events.CommandLog
{
    public class CommandBodyStoredLogEvent : CommandLogEvent
    {
        public string JsonBody { get; set; }
    }
}