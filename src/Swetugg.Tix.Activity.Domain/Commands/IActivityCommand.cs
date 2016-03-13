using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public interface IActivityCommand
    {
        Guid ActivityId { get; }
        Guid CommandId { get; }
    }
}