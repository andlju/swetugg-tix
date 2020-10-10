using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class ReturnSeatHandler : ActivityCommandHandler<ReturnSeat>
    {
        public ReturnSeatHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, ReturnSeat cmd)
        {
            activity.ReturnSeat(cmd.TicketTypeId, cmd.CouponId, cmd.Reference);
        }
    }
}