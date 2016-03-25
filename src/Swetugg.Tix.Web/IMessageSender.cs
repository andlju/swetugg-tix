using System.Threading.Tasks;

namespace Swetugg.Tix.Web
{
    public interface IMessageSender
    {
        Task Send(object message);
    }
}