using System;

namespace Swetugg.Tix.Order.Events
{
    public class SeatReserved : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public string TicketReference { get; set; }
    }
}