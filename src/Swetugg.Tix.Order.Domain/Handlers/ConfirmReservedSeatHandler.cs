using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{

    public class ConfirmReservedSeatHandler : OrderCommandHandler<ConfirmReservedSeat>
    {
        public ConfirmReservedSeatHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Order order, ConfirmReservedSeat cmd)
        {
            order.ConfirmReservedSeat(cmd.TicketTypeId, cmd.TicketReference);
        }
    }
}