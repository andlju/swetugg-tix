using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain.CommandLog;
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

        protected override Activity GetActivity(Guid activityId)
        {
            return new Activity(activityId);
        }
    }
}