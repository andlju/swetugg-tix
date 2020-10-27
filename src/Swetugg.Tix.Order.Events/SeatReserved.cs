using System;

namespace Swetugg.Tix.Order.Events
{
    public class SeatReserved : EventBase
    {
        public Guid TicketId { get; set; }
        public string TicketReference { get; set; }
    }
}