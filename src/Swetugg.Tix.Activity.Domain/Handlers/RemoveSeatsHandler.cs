using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveSeatsHandler : ActivityCommandHandler<RemoveSeats>
    {
        public RemoveSeatsHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveSeats cmd)
        {
            activity.RemoveSeats(cmd.Seats);
        }
    }
}