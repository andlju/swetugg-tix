using System;

namespace Swetugg.Tix.Activity.Commands.Admin
{

    public class RebuildViews : ActivityAdminCommand
    {
        public Guid OwnerId { get; set; }
        public Guid ActivityId { get; set; }
    }
}
