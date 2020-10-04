using System;

namespace Swetugg.Tix.Activity.Events
{
    public abstract class EventBase
    {
        public Guid AggregateId { get; set; }
    }
}