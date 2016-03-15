using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Infrastructure
{
    [Serializable]
    public class CommandHandlerException : Exception
    {
        public CommandHandlerException(string message) : base(message)
        {
        }

        public CommandHandlerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CommandHandlerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}