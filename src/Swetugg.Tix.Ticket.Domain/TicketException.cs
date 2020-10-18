using System;
using System.Runtime.Serialization;

namespace Swetugg.Tix.Ticket.Domain
{
    [Serializable]
    public class TicketException : Exception
    {
        public TicketException(string errorCode) : base($"{errorCode} error has occured")
        {
            ErrorCode = errorCode;
        }

        public TicketException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public TicketException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; set; }

        protected TicketException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}