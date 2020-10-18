using System;

namespace Swetugg.Tix.Order.Commands
{
    public class OrderCommand : IOrderCommand
    {
        public Guid OrderId { get; set; }
        public Guid CommandId { get; set; }
    }
}