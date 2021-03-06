using NEventStore.Domain.Persistence;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;
using System;
using System.Threading.Tasks;

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

        public async Task Handle(TCmd cmd)
        {
            if (cmd.OwnerId == Guid.Empty)
                throw new ArgumentNullException("OwnerId");

            await _commandLog.Store(cmd.CommandId, cmd, cmd.ActivityId.ToString());
            try
            {
                var activity = GetActivity(cmd);

                HandleCommand(activity, cmd);

                _repository.Save(cmd.OwnerId.ToString(), activity, Guid.NewGuid(), headers =>
                {
                    headers.Add("CommandId", cmd.CommandId.ToString());
                    headers.Add("UserId", cmd.Headers.UserId.ToString());
                });
                await _commandLog.Complete(cmd.CommandId, activity.Version);
            }
            catch (ActivityException ex)
            {
                // This is a domain error and shouldn't be retried
                await _commandLog.Fail(cmd.CommandId, ex.ErrorCode, ex.Message);
            }
            catch(Exception ex)
            {
                // This is an infrastructure error or bug. Let's try again a few times.
                await _commandLog.Fail(cmd.CommandId, "UnknownError", ex.ToString());
                throw;
            }
        }

        protected virtual Activity GetActivity(TCmd cmd)
        {
            var activity = _repository.GetById<Activity>(cmd.OwnerId.ToString(), cmd.ActivityId);
            if (activity.Id == Guid.Empty)
                throw new InvalidOperationException($"Unable to find Activity {cmd.ActivityId} with Owner {cmd.OwnerId}");

            return activity;
        }

        protected abstract void HandleCommand(Activity activity, TCmd cmd);
    }
}