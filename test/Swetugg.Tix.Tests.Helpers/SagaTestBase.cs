using NEventStore;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Swetugg.Tix.Tests.Helpers
{
    public abstract class SagaTestBase
    {
        private readonly GivenEventsImpl _givenInternal = new GivenEventsImpl();

        /// <summary>
        /// Used to setup any commands that are preconditions
        /// to this test
        /// </summary>
        protected IGivenEvents Given => _givenInternal;

        protected abstract IMessageDispatcher WithDispatcher(Wireup eventStoreWireup, ISagaMessageDispatcher sagaMessageDispatcher, IEnumerable<IPipelineHook> extraHooks);

        /// <summary>
        /// Commits that have been committed as a result of the
        /// command under test
        /// </summary>
        protected IEnumerable<ICommit> Commits => _commitsInternal;

        /// <summary>
        /// Commits that happened as part of the Given phase
        /// </summary>
        protected IEnumerable<ICommit> PreCommits => _preCommitsInternal;

        protected IEnumerable<object> DispatchedMessages => _dispatchedMessagesInternal;
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
        private readonly List<ICommit> _preCommitsInternal = new List<ICommit>();
        private readonly List<object> _dispatchedMessagesInternal = new List<object>();

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected SagaTestBase(ITestOutputHelper output)
        {
            Output = output;
            var testHook = new RepositoryTestObserver(_preCommitsInternal, _commitsInternal);
            var sagaMessageDispatcher = new TestSagaMessageDispatcher(_dispatchedMessagesInternal);
            // Setup an InMemory EventStore with a hook
            // for recording commits
            var eventStoreWireup = Wireup.Init()
                .UsingInMemoryPersistence();

            var dispatcher = WithDispatcher(eventStoreWireup, sagaMessageDispatcher, new[] { testHook });

            // Let the actual test setup any preconditions
            Setup();

            var givenEvents = _givenInternal.Events;
            foreach (var givenEvt in givenEvents)
            {
                dispatcher.Dispatch(givenEvt).Wait();
            }

            testHook.CollectCommits = true;
            sagaMessageDispatcher.CollectMessages = true;

            var evt = When();
            try
            {
                dispatcher.Dispatch(evt).Wait();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
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

        class GivenEventsImpl : IGivenEvents
        {
            private readonly List<object> _events = new List<object>();

            public void AddEvent(object evt)
            {
                _events.Add(evt);
            }

            public IEnumerable<object> Events => _events;
        }

    }
}