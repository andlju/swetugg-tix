using System.Collections.Generic;

namespace Swetugg.Tix.User.Contract
{
    public class PermissionClaim
    {
        public string PermissionCode { get; set; }
        public Dictionary<string,string> Attributes { get; set; }
    }
}
