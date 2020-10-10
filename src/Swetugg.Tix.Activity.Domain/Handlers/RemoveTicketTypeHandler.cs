using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveTicketTypeHandler : ActivityCommandHandler<RemoveTicketType>
    {
        public RemoveTicketTypeHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveTicketType cmd)
        {
            activity.RemoveTicketType(cmd.TicketTypeId);
        }
    }
}