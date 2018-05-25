using System;

namespace Swetugg.Tix.Activity.Events
{
    public class TicketTypeLimitRemoved : EventBase
    {
        public Guid TicketTypeId { get; set; }
    }
}