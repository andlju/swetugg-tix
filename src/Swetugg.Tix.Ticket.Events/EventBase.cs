using System;

namespace Swetugg.Tix.Ticket.Events
{
    public abstract class EventBase
    {
        public Guid AggregateId { get; set; }
    }
}