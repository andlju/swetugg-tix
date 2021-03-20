using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class CreateOrderWithTicketsHandler : OrderCommandHandler<CreateOrderWithTickets>
    {

        public CreateOrderWithTicketsHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Order order, CreateOrderWithTickets cmd)
        {
            if (cmd.Tickets == null)
                return;

            foreach(var ticket in cmd.Tickets)
            {
                for (int i = 0; i < ticket.Quantity; i++)
                {
                    order.AddTicket(ticket.TicketTypeId);
                }
            }
        }

        protected override Order GetOrder(CreateOrderWithTickets cmd)
        {
            return new Order(cmd.OrderId, cmd.ActivityId, cmd.ActivityOwnerId);
        }
    }
}