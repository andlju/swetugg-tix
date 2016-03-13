using System;

namespace Swetugg.Tix.Activity.Events
{
    public class TicketTypeLimitIncreased : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public int Seats { get; set; }
    }
}