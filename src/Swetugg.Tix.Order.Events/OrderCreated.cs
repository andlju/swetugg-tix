using System;

namespace Swetugg.Tix.Order.Events
{

    public class OrderCreated : EventBase
    {
        public Guid ActivityId { get; set; }
    }

}