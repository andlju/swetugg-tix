namespace Swetugg.Tix.Activity.Events.CommandLog
{
    public class CommandFailedLogEvent : CommandLogEvent
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}