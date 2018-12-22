using System.Threading.Tasks;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Tests.Helpers
{
    public class NullEventPublisher : IEventPublisher
    {
        public Task Publish(object evt)
        {
            return Task.FromResult(0);
        }
    }
}