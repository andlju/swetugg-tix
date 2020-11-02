using System;
using System.Collections.Generic;
using System.Text;

namespace Swetugg.Tix.Order.Commands.Admin
{

    public interface IOrderAdminCommand
    {
        Guid CommandId { get; }
    }
}
