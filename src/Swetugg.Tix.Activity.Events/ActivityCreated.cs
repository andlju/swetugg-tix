
using System;

namespace Swetugg.Tix.Activity.Events
{
    public class ActivityCreated : EventBase
    {
        public Guid CreatedByUserId { get; set; }
    }
}