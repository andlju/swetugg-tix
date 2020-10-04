using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Domain.CommandLog;
using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Activity.Domain.Handlers
{

    public abstract class ActivityCommandHandler<TCmd> :
        IMessageHandler<TCmd>
        where TCmd : IActivityCommand
    {
        private readonly IRepository _repository;
        private readonly ICommandLog _commandLog;

        protected ActivityCommandHandler(IRepository repository, ICommandLog commandLog)
        {
            _repository = repository;
            _commandLog = commandLog;
        }

        public void Handle(TCmd msg)
        {
            _commandLog.Store(msg);
            try
            {
                var activity = GetActivity(msg.ActivityId);

                HandleCommand(activity, msg);

                _repository.Save(activity, Guid.NewGuid(), headers =>
                {
                    headers.Add("CommandId", msg.CommandId.ToString());
                });
                _commandLog.Complete(msg.CommandId);
            }
            catch (ActivityException ex)
            {
                // This is a domain error and shouldn't be retried
                _commandLog.Fail(msg.CommandId, ex.ErrorCode, ex.Message);
            }
            catch(Exception ex)
            {
                // This is an infrastructure error or bug. Let's try again a few times.
                _commandLog.Fail(msg.CommandId, ex);
                throw;
            }
        }

        protected virtual Activity GetActivity(Guid activityId)
        {
            var activity = _repository.GetById<Activity>(activityId);
            return activity;
        }

        protected abstract void HandleCommand(Activity activity, TCmd cmd);
    }
}