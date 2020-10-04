using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain.CommandLog;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveTicketTypeLimitHandler : ActivityCommandHandler<RemoveTicketTypeLimit>
    {
        public RemoveTicketTypeLimitHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveTicketTypeLimit cmd)
        {
            activity.RemoveTicketTypeLimit(cmd.TicketTypeId);
        }
    }
}