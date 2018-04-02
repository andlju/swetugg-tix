using System;

namespace Swetugg.Tix.Infrastructure
{
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(string message) : base(message)
        {
        }

        public MessageHandlerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}