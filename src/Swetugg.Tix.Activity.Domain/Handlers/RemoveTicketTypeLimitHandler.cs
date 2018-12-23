using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveTicketTypeLimitHandler : ActivityCommandHandler<RemoveTicketTypeLimit>
    {
        public RemoveTicketTypeLimitHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveTicketTypeLimit cmd)
        {
            activity.RemoveTicketTypeLimit(cmd.TicketTypeId);
        }
    }
}