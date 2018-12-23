using System;
using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public abstract class ActivityCommandHandler<TCmd> : 
        IMessageHandler<TCmd>
        where TCmd : IActivityCommand
    {
        private readonly IRepository _repository;

        protected ActivityCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(TCmd msg)
        {
            var activity = GetActivity(msg.ActivityId);

            HandleCommand(activity, msg);
            
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