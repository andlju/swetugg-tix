using System;
using System.Collections.Generic;

namespace Swetugg.Tix.User.Contract
{
    public class UserRole
    {
        public Guid UserRoleId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<UserRoleAttribute> Attributes { get; set; }
    }
}
