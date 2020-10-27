using NEventStore;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Tests.Helpers
{
    public static class CommitTestExtensions
    {
        public static bool HasEvent<T>(this IEnumerable<ICommit> commits)
        {
            return commits.SelectMany(e => e.Events).Any(e => e.Body is T);
        }

        public static bool HasEvent<T>(this ICommit commit)
        {
            return commit.Events.Any(e => e.Body is T);
        }

        public static T GetEvent<T>(this ICommit commit, int number = 1)
        {
            return commit.GetEvents<T>().Skip(number - 1).FirstOrDefault();
        }

        public static T GetEvent<T>(this IEnumerable<ICommit> commits, int number = 1)
        {
            return commits.GetEvents<T>().Skip(number - 1).FirstOrDefault();
        }

        public static IEnumerable<T> GetEvents<T>(this ICommit commit)
        {
            return commit.Events.Select(e => e.Body).Where(e => e is T).Cast<T>();
        }

        public static IEnumerable<T> GetEvents<T>(this IEnumerable<ICommit> commits)
        {
            return commits.SelectMany(c => c.Events).Select(e => e.Body).Where(e => e is T).Cast<T>();
        }
    }
}