using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class AddTicketsHandler : OrderCommandHandler<AddTickets>
    {
        public AddTicketsHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {

        }

        protected override void HandleCommand(Order order, AddTickets cmd)
        {
            if (cmd.Tickets == null)
                return;

            foreach (var ticket in cmd.Tickets)
            {
                for (int i = 0; i < ticket.Quantity; i++)
                {
                    order.AddTicket(ticket.TicketTypeId);
                }
            }
        }
    }
}