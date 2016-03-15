using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public class CreateActivityHandler : ActivityCommandHandler<CreateActivity>
    {
        public CreateActivityHandler(IRepository repository) : base(repository)
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