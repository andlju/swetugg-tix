using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class ReserveSeatHandler : ActivityCommandHandler<ReserveSeat>
    {
        public ReserveSeatHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, ReserveSeat cmd)
        {
            activity.ReserveSeat(cmd.TicketTypeId, cmd.OrderReference);
        }
    }
}