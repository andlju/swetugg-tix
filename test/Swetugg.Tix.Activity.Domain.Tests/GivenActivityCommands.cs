using System;
using Swetugg.Tix.Activity.Domain.Commands;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class GivenActivityCommands 
    {
        private readonly IGivenCommands _parent;
        private readonly Guid _activityId;

        public GivenActivityCommands(IGivenCommands parent, Guid activityId)
        {
            _parent = parent;
            _activityId = activityId;
        }

        public void AddCommand(ActivityCommand cmd)
        {
            cmd.ActivityId = _activityId;
            _parent.AddCommand(cmd);
        }
    }
}