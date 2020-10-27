using System;

namespace Swetugg.Tix.Order.Events
{
    public class SeatReturned : EventBase
    {
        public Guid TicketId { get; set; }
        public string TicketReference { get; set; }
    }
}