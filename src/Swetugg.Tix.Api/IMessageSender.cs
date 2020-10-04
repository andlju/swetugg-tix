using System.Threading.Tasks;

namespace Swetugg.Tix.Api
{
    public interface IMessageSender
    {
        Task Send(object message);
        Task Close();
    }
}