using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface IMessageHandler<T>
    {
        Task Handle(T msg);
    }
}