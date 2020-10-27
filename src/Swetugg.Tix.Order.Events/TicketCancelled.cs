using System;

namespace Swetugg.Tix.Order.Events
{
    public class TicketCancelled : EventBase
    {
        public Guid TicketId { get; set; }
    }
}