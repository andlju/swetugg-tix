using System;

namespace Swetugg.Tix.Activity.Events
{
    public class TicketTypeAdded : EventBase
    {
        public Guid TicketTypeId { get; set; }
    }
}