using System.Linq;
using NEventStore;

namespace Swetugg.Tix.Tests.Helpers
{
    public static class CommitTestExtensions
    {
        public static bool HasEvent<T>(this ICommit commit)
        {
            return commit.Events.Any(e => e.Body is T);
        }
    }
}