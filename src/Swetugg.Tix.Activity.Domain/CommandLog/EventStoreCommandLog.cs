using NEventStore;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Activity.Events.CommandLog;
using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Activity.Domain.CommandLog
{
    public class EventStoreCommandLog : ICommandLog
    {
        private IStoreEvents _eventStore;

        public EventStoreCommandLog(IStoreEvents eventStore)
        {
            _eventStore = eventStore;
        }

        private void GuardEventStore()
        {
            if (_eventStore == null)
            {
                throw new InvalidOperationException("EventStore property has not been set on the EventStoreCommandLog");
            }
        }

        IEventStream GetOrCreateStream(Guid commandId)
        {
            GuardEventStore();
            IEventStream stream;
            try
            {
                stream = _eventStore.OpenStream(commandId);
            }
            catch (StreamNotFoundException)
            {
                stream = _eventStore.CreateStream(commandId);
            }

            return stream;
        }

        void StoreCommandEvent(CommandLogEvent evt)
        {
            var stream = GetOrCreateStream(evt.CommandId);
            stream.Add(new EventMessage()
            {
                Body = evt,
                Headers = new Dictionary<string, object>
                {
                    ["CommandLog"] = true
                }
            });
            stream.CommitChanges(Guid.NewGuid());
        }

        public void Store<TCmd>(TCmd command) where TCmd : IActivityCommand
        {
            StoreCommandEvent(new CommandBodyStoredLogEvent()
            {
                CommandId = command.CommandId,
                ActivityId = command.ActivityId,
                JsonBody = System.Text.Json.JsonSerializer.Serialize(command)
            });
        }

        public void Complete(Guid commandId)
        {
            StoreCommandEvent(new CommandCompletedLogEvent()
            {
                CommandId = commandId,
            });
        }

        public void Fail(Guid commandId, Exception ex)
        {
            // TODO If ActivityException, fetch code etc
            StoreCommandEvent(new CommandFailedLogEvent()
            {
                CommandId = commandId,
                Code = "CommandFailed",
                Message = "Command failed for an unknown reason"
            });
        }

        public void Fail(Guid commandId, string code, string message)
        {
            // TODO If ActivityException, fetch code etc
            StoreCommandEvent(new CommandFailedLogEvent()
            {
                CommandId = commandId,
                Code = code,
                Message = message
            });
        }
    }
}