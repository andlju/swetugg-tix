using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class ActivityCommand : IActivityCommand
    {
        public Guid ActivityId { get; set; }
        public Guid CommandId { get; set; }
    }
}