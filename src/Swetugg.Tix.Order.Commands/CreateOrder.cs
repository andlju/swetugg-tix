using System;

namespace Swetugg.Tix.Order.Commands
{
    public class CreateOrder : OrderCommand
    {
        public Guid ActivityId { get; set; }
    }

}