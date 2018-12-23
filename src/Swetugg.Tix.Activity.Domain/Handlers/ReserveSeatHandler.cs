using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class ReserveSeatHandler : ActivityCommandHandler<ReserveSeat>
    {
        public ReserveSeatHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, ReserveSeat cmd)
        {
            activity.ReserveSeat(cmd.TicketTypeId, cmd.CouponId, cmd.Reference);
        }
    }
}