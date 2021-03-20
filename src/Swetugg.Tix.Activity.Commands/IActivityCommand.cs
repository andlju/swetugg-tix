using System;

namespace Swetugg.Tix.Activity.Commands
{

    public interface IActivityCommand
    {
        Guid ActivityId { get; }
        Guid OwnerId { get; }
        Guid CommandId { get; }
        CommandHeaders Headers { get; }
        int ExpectedRevision { get; }
    }
}