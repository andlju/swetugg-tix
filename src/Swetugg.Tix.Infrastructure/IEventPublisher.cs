using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface IEventPublisher
    {
        Task Publish(object evt, string aggregateId);
    }
}