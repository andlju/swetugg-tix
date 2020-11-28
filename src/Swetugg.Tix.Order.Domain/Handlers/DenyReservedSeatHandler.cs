using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class DenyReservedSeatHandler : OrderCommandHandler<DenyReservedSeat>
    {
        public DenyReservedSeatHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Order order, DenyReservedSeat cmd)
        {
            order.DenyReservedSeat(cmd.TicketTypeId, cmd.ReasonCode);
        }
    }
}