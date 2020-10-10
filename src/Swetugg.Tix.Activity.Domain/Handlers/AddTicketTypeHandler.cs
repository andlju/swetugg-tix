using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class AddTicketTypeHandler : ActivityCommandHandler<AddTicketType>
    {
        public AddTicketTypeHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, AddTicketType cmd)
        {
            activity.AddTicketType(cmd.TicketTypeId);
        }
    }
}