using System;

namespace Swetugg.Tix.Order.Events
{
    public class TicketCancelled : EventBase
    {
        public Guid TicketTypeId { get; set; }
    }
}