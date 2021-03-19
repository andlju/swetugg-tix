using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class ActivityCommand : IActivityCommand
    {
        public Guid ActivityId { get; set; }
        public Guid CommandId { get; set; }
        public Guid UserId { get; set; }
        public int ExpectedRevision { get; set; }
    }
}