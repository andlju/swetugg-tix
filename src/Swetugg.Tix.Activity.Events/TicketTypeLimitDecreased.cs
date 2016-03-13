using System;

namespace Swetugg.Tix.Activity.Events
{
    public class TicketTypeLimitDecreased : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public int Seats { get; set; }
    }
}