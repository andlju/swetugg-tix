using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Activity.Domain.Commands;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public abstract class ActivityCommandHandler<TCmd> : 
        ICommandHandler<TCmd>
        where TCmd : IActivityCommand
    {
        private readonly IRepository _repository;

        protected ActivityCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(TCmd cmd)
        {
            var activity = GetActivity(cmd.ActivityId);

            HandleCommand(activity, cmd);
            
            _repository.Save(activity, Guid.NewGuid());
        }

        protected virtual Activity GetActivity(Guid activityId)
        {
            var activity = _repository.GetById<Activity>(activityId);
            return activity;
        }

        protected abstract void HandleCommand(Activity activity, TCmd cmd);
    }
}