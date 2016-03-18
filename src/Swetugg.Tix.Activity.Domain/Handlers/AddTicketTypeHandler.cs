using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class AddTicketTypeHandler : ActivityCommandHandler<AddTicketType>
    {
        public AddTicketTypeHandler(IRepository repository) : base(repository)
        {
        }

        protected override void HandleCommand(Activity activity, AddTicketType cmd)
        {
            activity.AddTicketType(cmd.TicketTypeId);
        }
    }
}