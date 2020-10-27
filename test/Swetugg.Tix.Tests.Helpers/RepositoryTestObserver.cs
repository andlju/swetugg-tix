using NEventStore;
using System.Collections.Generic;

namespace Swetugg.Tix.Tests.Helpers
{
    class RepositoryTestObserver : PipelineHookBase
    {
        private readonly ICollection<ICommit> _preCommits;
        private readonly ICollection<ICommit> _commits;

        public RepositoryTestObserver(ICollection<ICommit> preCommits, ICollection<ICommit> commits)
        {
            _preCommits = preCommits;
            _commits = commits;
        }

        public override void PostCommit(ICommit committed)
        {
            if (CollectCommits)
            {
                _commits.Add(committed);
            }
            else
            {
                _preCommits.Add(committed);
            }
        }

        public bool CollectCommits;
    }
}