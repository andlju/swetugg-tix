using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class IncreaseTicketTypeLimitHandler : ActivityCommandHandler<IncreaseTicketTypeLimit>
    {
        public IncreaseTicketTypeLimitHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, IncreaseTicketTypeLimit cmd)
        {
            activity.IncreaseTicketTypeLimit(cmd.TicketTypeId, cmd.Seats);
        }
    }
}