using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Swetugg.Tix.User
{

    public class ObjectAuthorizationManager
    {
        readonly Dictionary<string, PermissionClaim[]> _claims;

        public ObjectAuthorizationManager(IEnumerable<PermissionClaim> claims)
        {
            _claims = claims.GroupBy(c => c.PermissionCode, StringComparer.InvariantCultureIgnoreCase).ToDictionary(c => c.Key, c => c.ToArray());
        }

        public bool HasPermissionForObject(string permissionName, object objectToAuthorize)
        {
            if (!_claims.TryGetValue(permissionName, out var claims))
                return false;

            var objType = objectToAuthorize.GetType();
            foreach(var claim in claims)
            {
                bool claimValid = true;
                foreach (var attribute in claim.Attributes)
                {
                    if (attribute.Value == "*")
                        continue;
                    
                    // TODO Optimize this use of reflection
                    var prop = objType.GetProperty(attribute.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop == null)
                        throw new InvalidOperationException($"No property called {attribute.Name} found on object to authorize");

                    var propValue = prop.GetValue(objectToAuthorize);
                    if (propValue == null)
                        throw new InvalidOperationException("Found null value in property to authorize");

                    if (propValue.ToString() != attribute.Value)
                    {
                        claimValid = false;
                        break;
                    }
                }
                if (claimValid)
                    return true;
            }
            // No claim has been found valid
            return false;
        }
    }
}
