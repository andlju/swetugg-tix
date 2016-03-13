using System;

namespace Swetugg.Tix.Activity.Events
{
    public class TicketTypeRemoved : EventBase
    {
        public Guid TicketTypeId { get; set; }
    }
}