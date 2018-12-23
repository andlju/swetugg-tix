using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class IncreaseTicketTypeLimitHandler : ActivityCommandHandler<IncreaseTicketTypeLimit>
    {
        public IncreaseTicketTypeLimitHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, IncreaseTicketTypeLimit cmd)
        {
            activity.IncreaseTicketTypeLimit(cmd.TicketTypeId, cmd.Seats);
        }
    }
}