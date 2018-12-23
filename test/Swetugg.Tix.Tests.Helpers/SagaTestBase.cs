using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NEventStore;
using Swetugg.Tix.Infrastructure;
using Xunit.Abstractions;

namespace Swetugg.Tix.Tests.Helpers
{
    public abstract class SagaTestBase
    {
        protected abstract IMessageDispatcher WithDispatcher(Wireup eventStoreWireup, IEnumerable<IPipelineHook> extraHooks);

        /// <summary>
        /// Exception that has been thrown as a result of the
        /// command under test
        /// </summary>
        protected Exception ThrownException = null;

        /// <summary>
        /// Use this for logging test output
        /// </summary>
        protected readonly ITestOutputHelper Output;

        private readonly List<ICommit> _commitsInternal = new List<ICommit>();

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected SagaTestBase(ITestOutputHelper output)
        {
            Output = output;
            var testHook = new RepositoryTestObserver(_commitsInternal);
            // Setup an InMemory EventStore with a hook
            // for recording commits
            var eventStoreWireup = Wireup.Init()
                .UsingInMemoryPersistence();

            var dispatcher = WithDispatcher(eventStoreWireup, new[] { testHook });

            // Let the actual test setup any preconditions
            Setup();

            
        }

        /// <summary>
        /// Override to setup any preconditions
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Override to return the command under test
        /// </summary>
        /// <returns></returns>
        protected abstract object When();

    }
}