using System;

namespace Swetugg.Tix.Order.Commands
{
    public interface IOrderCommand
    {
        Guid OrderId { get; }
        Guid CommandId { get; }
    }
}