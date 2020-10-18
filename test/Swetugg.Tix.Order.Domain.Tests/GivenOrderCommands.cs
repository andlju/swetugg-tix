using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Tests.Helpers;
using System;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class GivenOrderCommands
    {
        private readonly IGivenCommands _parent;
        private readonly Guid _orderId;

        public GivenOrderCommands(IGivenCommands parent, Guid orderId)
        {
            _parent = parent;
            _orderId = orderId;
        }

        public Guid OrderId => _orderId;

        public void AddCommand(OrderCommand cmd)
        {
            cmd.OrderId = _orderId;
            _parent.AddCommand(cmd);
        }
    }
}