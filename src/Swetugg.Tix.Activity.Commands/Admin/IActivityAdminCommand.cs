using System;
using System.Collections.Generic;
using System.Text;

namespace Swetugg.Tix.Activity.Commands.Admin
{

    public interface IActivityAdminCommand
    {
        Guid CommandId { get; }
    }
}
