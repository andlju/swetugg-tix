using Swetugg.Tix.Activity.Commands;
using System;

namespace Swetugg.Tix.Activity.Domain.CommandLog
{
    public interface ICommandLog
    {
        void Store<TCmd>(TCmd command) where TCmd : IActivityCommand;
        void Complete(Guid commandId);
        void Fail(Guid commandId, Exception ex);
        void Fail(Guid commandId, string code, string message);
    }
}