using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class DecreaseTicketTypeLimitHandler : ActivityCommandHandler<DecreaseTicketTypeLimit>
    {
        public DecreaseTicketTypeLimitHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, DecreaseTicketTypeLimit cmd)
        {
            activity.DecreaseTicketTypeLimit(cmd.TicketTypeId, cmd.Seats);
        }
    }
}