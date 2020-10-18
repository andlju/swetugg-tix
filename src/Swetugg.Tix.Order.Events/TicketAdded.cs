using System;

namespace Swetugg.Tix.Order.Events
{
    public class TicketAdded : EventBase
    {
        public Guid TicketTypeId { get; set; }
    }
}