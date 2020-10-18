using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Order.Domain
{
    [Serializable]
    public class OrderException : Exception
    {
        public OrderException(string errorCode) : base($"{errorCode} error has occured")
        {
            ErrorCode = errorCode;
        }

        public OrderException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public OrderException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; set; }

        protected OrderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}