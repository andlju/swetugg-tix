using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventStore;
using Swetugg.Tix.Ticket.Events;
using Xunit;

namespace Swetugg.Tix.Process.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Test
    {
        public Test()
        {
        }

        protected List<object> DispatchedMessages = new List<object>();

        [Fact]
        public void test()
        {
            // Setup an InMemory EventStore with a hook
            // for recording commits
            var eventStoreWireup = Wireup.Init()
                .UsingInMemoryPersistence();

            var host = new Host(eventStoreWireup, new TestDispatcher(this));
            host.HandleEvent(new TicketCreated());

            Assert.NotEmpty(DispatchedMessages);
        }

        class TestDispatcher : IMessageDispatcher
        {
            private readonly Test _parent;

            public TestDispatcher(Test parent)
            {
                _parent = parent;
            }

            public void Dispatch(object message)
            {
                _parent.DispatchedMessages.Add(message);
            }
        }
    }
}