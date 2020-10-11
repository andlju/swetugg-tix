using Swetugg.Tix.Activity.Commands.Admin;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers.Admin
{
    public abstract class AdminCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : IActivityAdminCommand
    {
        public abstract void Handle(TCmd command);
    }
}