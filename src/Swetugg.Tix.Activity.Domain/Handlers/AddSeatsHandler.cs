using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain.CommandLog;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class AddSeatsHandler : ActivityCommandHandler<AddSeats>
    {
        public AddSeatsHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, AddSeats cmd)
        {
            activity.AddSeats(cmd.Seats);
        }
    }
}