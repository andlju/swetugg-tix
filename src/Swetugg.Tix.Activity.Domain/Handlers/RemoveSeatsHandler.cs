using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

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