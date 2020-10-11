using System;

namespace Swetugg.Tix.Activity.Commands.Admin
{
    public abstract class ActivityAdminCommand : IActivityAdminCommand
    {
        public Guid CommandId { get; set; }
    }
}
