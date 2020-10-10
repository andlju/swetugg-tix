using NEventStore;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Swetugg.Tix.Tests.Helpers
{
    /// <summary>
    /// Base class for all domain tests
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class AggregateTestBase
    {
        private readonly GivenCommandsImpl _givenInternal = new GivenCommandsImpl();
        private readonly TestCommandLog _testCommandLog = new TestCommandLog();

        /// <summary>
        /// Used to setup any commands that are preconditions
        /// to this test
        /// </summary>
        protected IGivenCommands Given => _givenInternal;

        private readonly List<ICommit> _commitsInternal = new List<ICommit>();

        /// <summary>
        /// Commits that have been committed as a result of the
        /// command under test
        /// </summary>
        protected IEnumerable<ICommit> Commits => _commitsInternal;

        protected TestCommandLog Command => _testCommandLog;

        /// <summary>
        /// Exception that has been thrown as a result of the
        /// command under test
        /// </summary>
        protected Exception ThrownException = null;

        /// <summary>
        /// Use this for logging test output
        /// </summary>
        protected readonly ITestOutputHelper Output;

        class GivenCommandsImpl : IGivenCommands
        {
            private readonly List<object> _commands = new List<object>();

            public void AddCommand(object cmd)
            {
                _commands.Add(cmd);
            }

            public IEnumerable<object> Commands => _commands;
        }

        protected class TestCommandLog : ICommandLog
        {
            public bool HasCompleted { get; private set; }
            public bool HasFailed { get; private set; }

            public string FailureCode { get; private set; }
            public string FailureMessage { get; private set; }

            public Task Complete(Guid commandId)
            {
                HasCompleted = true;
                return Task.FromResult(0);
            }

            public Task Fail(Guid commandId, string code, string message)
            {
                HasFailed = true;
                FailureCode = code;
                FailureMessage = message;
                return Task.FromResult(0);
            }

            public Task Store(Guid commandId, object command, string aggregateId = null)
            {
                return Task.FromResult(0);
            }
        }

        protected abstract IMessageDispatcher WithDispatcher(Wireup eventStoreWireup, IEnumerable<IPipelineHook> extraHooks, ICommandLog commandLog);

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Overriding classes should not implement their own constructors")]
        protected AggregateTestBase(ITestOutputHelper output)
        {
            Output = output;
            var testHook = new RepositoryTestObserver(_commitsInternal);
            // Setup an InMemory EventStore with a hook
            // for recording commits
            var eventStoreWireup = Wireup.Init()
                .UsingInMemoryPersistence();

            var dispatcher = WithDispatcher(eventStoreWireup, new[] { testHook }, _testCommandLog);

            // Let the actual test setup any preconditions
            Setup();

            // Dispatch all commands that should be preconditions
            foreach (var givenCommand in _givenInternal.Commands)
            {
                dispatcher.Dispatch(givenCommand);
            }

            // Make sure the test hook starts recording commits
            testHook.CollectCommits = true;

            // Get the command under test
            var whenCommand = When();
            try
            {
                // Dispatch the command
                dispatcher.Dispatch(whenCommand);
            }
            catch (Exception ex)
            {
                // If any exception is thrown when dispatching
                // the command, store it for testing
                ThrownException = ex;
                Output.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Override to setup any preconditions
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Overide to return the command under test
        /// </summary>
        /// <returns></returns>
        protected abstract object When();
    }
}