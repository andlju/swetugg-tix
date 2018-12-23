using System;
using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class AddSeatsHandler : ActivityCommandHandler<AddSeats>
    {
        public AddSeatsHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, AddSeats cmd)
        {
            activity.AddSeats(cmd.Seats);
        }
    }
}