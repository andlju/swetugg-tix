using System.Collections.Generic;
using NEventStore;

namespace Swetugg.Tix.Tests.Helpers
{
    class RepositoryTestObserver : PipelineHookBase
    {
        private readonly ICollection<ICommit> _commits;

        public RepositoryTestObserver(ICollection<ICommit> commits)
        {
            _commits = commits;
        }

        public override void PostCommit(ICommit committed)
        {
            if (CollectCommits)
            {
                _commits.Add(committed);
            }
        }

        public bool CollectCommits;
    }
}