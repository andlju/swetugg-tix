using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Activity.Domain
{
    [Serializable]
    public class ActivityException : Exception
    {
        public ActivityException(string errorCode) : base($"{errorCode} error has occured")
        {
            ErrorCode = errorCode;
        }

        public ActivityException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ActivityException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; set; }

        protected ActivityException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}