using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface ISagaMessageDispatcher
    {
        Task Dispatch(object message);
    }
}