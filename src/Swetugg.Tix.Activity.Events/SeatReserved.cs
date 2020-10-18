using System;

namespace Swetugg.Tix.Activity.Events
{
    public class SeatReserved : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public string OrderReference { get; set; }
        public string TicketReference { get; set; }
    }
}