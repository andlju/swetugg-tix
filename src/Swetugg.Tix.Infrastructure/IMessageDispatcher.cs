using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface IMessageDispatcher
    {
        Task Dispatch(object msg, bool throwOnMissing = true);
    }
}