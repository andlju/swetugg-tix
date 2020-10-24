using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.Infrastructure
{
    public interface IViewBuilder
    {
        Task HandleEvents(IEnumerable<PublishedEvent> events);
    }
}