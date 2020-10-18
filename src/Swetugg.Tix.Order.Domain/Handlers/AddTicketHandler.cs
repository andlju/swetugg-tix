using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class AddTicketHandler : OrderCommandHandler<AddTicket>
    {
        public AddTicketHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {

        }

        protected override void HandleCommand(Order order, AddTicket cmd)
        {
            order.AddTicket(cmd.TicketTypeId);
        }
    }
}