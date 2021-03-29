using System.Collections.Generic;

namespace Swetugg.Tix.User.Contract
{
    public class PermissionAttribute
    {
        public string Name { get; set; }
    }

    public class Permission
    {
        public string PermissionCode { get; set; }
        public List<PermissionAttribute> Attributes { get; set; }
    }
}
