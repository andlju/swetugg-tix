using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.ViewBuilder
{
  public interface IHandleEvent<TEvent>
    {
        Task Handle(TEvent evt);
    }
}