using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class CreateActivityHandler : ActivityCommandHandler<CreateActivity>
    {
        public CreateActivityHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Activity activity, CreateActivity cmd)
        {

        }

        protected override Activity GetActivity(CreateActivity cmd)
        {
            return new Activity(cmd.ActivityId, cmd.OwnerId);
        }
    }
}