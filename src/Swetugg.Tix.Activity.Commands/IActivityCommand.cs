using System;

namespace Swetugg.Tix.Activity.Commands
{
    public interface IActivityCommand
    {
        Guid ActivityId { get; }
        Guid CommandId { get; }
        Guid UserId { get; }
        int ExpectedRevision { get; }
    }
}