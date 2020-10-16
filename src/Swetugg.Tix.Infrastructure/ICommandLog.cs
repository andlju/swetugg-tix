using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface ICommandLog
    {
        Task Store(Guid commandId, object command, string aggregateId = null);
        Task Complete(Guid commandId, int? revision = null);
        Task Fail(Guid commandId, string code, string message);
    }
}
