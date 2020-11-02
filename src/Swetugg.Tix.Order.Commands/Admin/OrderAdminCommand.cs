using System;

namespace Swetugg.Tix.Order.Commands.Admin
{
    public abstract class OrderAdminCommand : IOrderAdminCommand
    {
        public Guid CommandId { get; set; }
    }
}
