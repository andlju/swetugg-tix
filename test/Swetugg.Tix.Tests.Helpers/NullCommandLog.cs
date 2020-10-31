using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Tests.Helpers
{
    public class NullCommandLog : ICommandLog
    {
        public Task Complete(Guid commandId, int? revision)
        {
            return Task.FromResult(0);
        }

        public Task Fail(Guid commandId, string code, string message)
        {
            return Task.FromResult(0);
        }

        public Task<CommandLogItem> GetCommandLog(Guid commandId)
        {
            throw new NotImplementedException();
        }

        public Task Store(Guid commandId, object command, string aggregateId = null)
        {
            return Task.FromResult(0);
        }
    }
}