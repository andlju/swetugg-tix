using System;

namespace Swetugg.Tix.Order.Events
{
    public class SeatReturned : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public string TicketReference { get; set; }
    }
}