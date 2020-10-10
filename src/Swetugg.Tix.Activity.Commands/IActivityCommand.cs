using System;

namespace Swetugg.Tix.Activity.Commands
{
    public interface IActivityCommand
    {
        Guid ActivityId { get; }
        Guid CommandId { get; }
        int ExpectedVersion { get; }
    }
}