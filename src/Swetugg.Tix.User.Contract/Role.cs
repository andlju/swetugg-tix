using System;
using System.Collections.Generic;

namespace Swetugg.Tix.User.Contract
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
