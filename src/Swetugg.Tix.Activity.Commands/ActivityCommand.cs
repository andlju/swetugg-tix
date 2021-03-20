using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class CommandHeaders
    {
        public Guid UserId { get; set; }
    }

    public class ActivityCommand : IActivityCommand
    {
        public Guid ActivityId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CommandId { get; set; }
        public CommandHeaders Headers { get; set; } = new CommandHeaders();
        public int ExpectedRevision { get; set; }
    }
}