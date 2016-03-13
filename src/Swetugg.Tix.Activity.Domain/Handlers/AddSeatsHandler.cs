using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

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