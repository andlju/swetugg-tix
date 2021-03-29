using System.Collections.Generic;

namespace Swetugg.Tix.User.Contract
{
    public class PermissionAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PermissionClaim
    {
        public string PermissionCode { get; set; }
        public List<PermissionAttribute> Attributes { get; set; }
    }
}
