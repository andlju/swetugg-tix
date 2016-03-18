using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class ReturnSeatHandler : ActivityCommandHandler<ReturnSeat>
    {
        public ReturnSeatHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, ReturnSeat cmd)
        {
            activity.ReturnSeat(cmd.TicketTypeId, cmd.CouponId, cmd.Reference);
        }
    }
}