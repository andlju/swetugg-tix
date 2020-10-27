using System;

namespace Swetugg.Tix.Order.Events
{
    public class TicketAdded : EventBase
    {
        public Guid TicketId { get; set; }
        public Guid TicketTypeId { get; set; }
    }
}