using System;

namespace Swetugg.Tix.Order.Events
{
    public class SeatDenied : EventBase
    {
        public Guid TicketId { get; set; }
        public string ReasonCode { get; set; }
    }
}