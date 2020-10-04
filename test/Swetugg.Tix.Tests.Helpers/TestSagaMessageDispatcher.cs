using Swetugg.Tix.Infrastructure;
using System.Collections.Generic;

namespace Swetugg.Tix.Tests.Helpers
{
    class TestSagaMessageDispatcher : ISagaMessageDispatcher
    {
        private readonly List<object> _dispatchedMessages;
        public bool CollectMessages;

        public TestSagaMessageDispatcher(List<object> dispatchedMessages)
        {
            _dispatchedMessages = dispatchedMessages;
        }

        public void Dispatch(object message)
        {
            if (CollectMessages)
            {
                _dispatchedMessages.Add(message);
            }
        }
    }
}