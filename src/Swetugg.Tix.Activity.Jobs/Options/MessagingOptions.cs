namespace Swetugg.Tix.Activity.Jobs.Options
{
    public class TopicOption
    {
        public string TopicName { get; set; }
    }

    public class MessagingOptions
    {
        public TopicOption EventPublisherTopic { get; set; }
    }
}