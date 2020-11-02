using System;

namespace Swetugg.Tix.Order.Commands.Admin
{

    public class RebuildViews : OrderAdminCommand
    {
        public Guid OrderId { get; set; }
    }
}
