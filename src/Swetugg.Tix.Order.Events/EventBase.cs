using System;

namespace Swetugg.Tix.Order.Events
{
    public abstract class EventBase
    {
        public Guid AggregateId { get; set; }
    }
}