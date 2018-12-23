using System;
using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveSeatsHandler : ActivityCommandHandler<RemoveSeats>
    {
        public RemoveSeatsHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveSeats cmd)
        {
            activity.RemoveSeats(cmd.Seats);
        }
    }
}