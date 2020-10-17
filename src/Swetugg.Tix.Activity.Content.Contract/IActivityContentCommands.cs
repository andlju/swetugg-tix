using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.Content.Contract
{
    public interface IActivityContentCommands
    {
        Task StoreActivityContent(ActivityContent content);
        Task StoreTicketTypeContent(TicketTypeContent ticketTypeContent);
    }
}
