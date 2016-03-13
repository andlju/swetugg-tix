using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

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