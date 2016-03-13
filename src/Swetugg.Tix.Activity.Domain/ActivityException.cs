using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Activity.Domain
{
    [Serializable]
    public class ActivityException : Exception
    {
        public ActivityException()
        {
        }

        public ActivityException(string message) : base(message)
        {
        }

        public ActivityException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ActivityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}