using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class DecreaseTicketTypeLimitHandler : ActivityCommandHandler<DecreaseTicketTypeLimit>
    {
        public DecreaseTicketTypeLimitHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, DecreaseTicketTypeLimit cmd)
        {
            activity.DecreaseTicketTypeLimit(cmd.TicketTypeId, cmd.Seats);
        }
    }
}