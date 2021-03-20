using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Tests.Helpers;
using System;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class GivenActivityCommands
    {
        private readonly IGivenCommands _parent;
        private readonly Guid _activityId;
        private readonly Guid _userId;
        private readonly Guid _ownerId;

        public GivenActivityCommands(IGivenCommands parent, Guid activityId, Guid userId, Guid ownerId)
        {
            _parent = parent;
            _activityId = activityId;
            _userId = userId;
            _ownerId = ownerId;
        }

        public Guid ActivityId => _activityId;

        public void AddCommand(ActivityCommand cmd)
        {
            cmd.OwnerId = _ownerId;
            cmd.ActivityId = _activityId;
            cmd.Headers.UserId = _userId;
            _parent.AddCommand(cmd);
        }
    }
}