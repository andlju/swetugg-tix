using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swetugg.Tix.User
{
    public class UserAuthorization
    {
        readonly List<PermissionClaim> _claims;

        public UserAuthorization(IEnumerable<PermissionClaim> claims)
        {
            _claims = claims.ToList();
        }

        public bool HasPermissionForObject(string permissionName, string objectType, object objectIds)
        {
            return false;
        }
    }
}
