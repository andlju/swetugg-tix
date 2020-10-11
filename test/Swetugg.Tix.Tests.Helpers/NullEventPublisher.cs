using Swetugg.Tix.Infrastructure;
using System.Threading.Tasks;

namespace Swetugg.Tix.Tests.Helpers
{
    public class NullEventPublisher : IEventPublisher
    {
        public Task Publish(PublishedEvents evts)
        {
            return Task.FromResult(0);
        }
    }
}