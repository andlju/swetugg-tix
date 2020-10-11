using System;

namespace Swetugg.Tix.Activity.Events.Admin
{
    public class RebuildViewsRequested : AdminEventBase
    {
        public Guid AggregateId { get; set; }
    }
}
