using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class RemoveTicketTypeHandler : ActivityCommandHandler<RemoveTicketType>
    {
        public RemoveTicketTypeHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, RemoveTicketType cmd)
        {
            activity.RemoveTicketType(cmd.TicketTypeId);
        }
    }
}