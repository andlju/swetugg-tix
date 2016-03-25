namespace Swetugg.Tix.Web.Options
{
    public class QueueOption
    {
        public string QueueName { get; set; }
    }

    public class MessagingOptions
    {
        public QueueOption CommandDispatchQueue { get; set; }
    }
}