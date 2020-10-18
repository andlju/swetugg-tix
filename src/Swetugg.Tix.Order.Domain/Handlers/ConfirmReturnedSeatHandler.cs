using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class ConfirmReturnedSeatHandler : OrderCommandHandler<ConfirmReturnedSeat>
    {
        public ConfirmReturnedSeatHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Order order, ConfirmReturnedSeat cmd)
        {
            order.ConfirmReturnedSeat(cmd.TicketTypeId, cmd.TicketReference);
        }
    }
}