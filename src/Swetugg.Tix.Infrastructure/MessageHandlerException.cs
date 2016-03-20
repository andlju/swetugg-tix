using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Infrastructure
{
    [Serializable]
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(string message) : base(message)
        {
        }

        public MessageHandlerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MessageHandlerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}