﻿namespace Swetugg.Tix.Ticket.Funcs.Options
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