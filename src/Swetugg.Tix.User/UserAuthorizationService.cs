using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IUserQueries _userQueries;
        private readonly IUserCommands _userCommands;

        public UserAuthorizationService(IUserQueries userQueries, IUserCommands userCommands)
        {
            _userCommands = userCommands;
            _userQueries = userQueries;
        }

        private Dictionary<Guid, Role> _rolesById;
        private Dictionary<string, Role> _rolesByName;

        protected Role GetRoleById(Guid roleId)
        {
            if (_rolesById == null)
                throw new InvalidOperationException("GetRoleById called before EnsureRoles");

            if (!_rolesById.TryGetValue(roleId, out var role))
                return null;

            return role;
        }
        protected Role GetRoleByName(string roleName)
        {
            if (_rolesByName == null)
                throw new InvalidOperationException("GetRoleByName called before EnsureRoles");

            if (!_rolesByName.TryGetValue(roleName, out var role))
                return null;

            return role;
        }

        private async Task EnsureRoles()
        {
            if (_rolesById != null && _rolesByName != null)
                return;

            var roles = (await _userQueries.ListRoles()).ToArray();
            _rolesById = roles.ToDictionary(r => r.RoleId);
            _rolesByName = roles.ToDictionary(r => r.Name);
        }

        public async Task AddUserRoleByName(Guid userId, string roleName, IEnumerable<UserRoleAttribute> attributes)
        {
            await EnsureRoles();
            var role = GetRoleByName(roleName);
            await AddUserRole(userId, role, attributes);
        }


        public async Task AddUserRoleById(Guid userId, Guid roleId, IEnumerable<UserRoleAttribute> attributes)
        {
            await EnsureRoles();
            var role = GetRoleById(roleId);
            await AddUserRole(userId, role, attributes);
        }

        private async Task AddUserRole(Guid userId, Role role, IEnumerable<UserRoleAttribute> attributes)
        {
            foreach(var rolePermission in role.Permissions)
            {
                foreach(var attrib in rolePermission.Attributes)
                {
                    if (!attributes.Any(a => a.Name == attrib.Name && !string.IsNullOrWhiteSpace(a.Value)))
                        throw new InvalidOperationException($"Please specify a value for the {attrib.Name} attribute");
                }    
            }
            await _userCommands.AddUserRole(userId, role.RoleId, attributes);
        }

        public async Task RemoveUserRole(Guid userId, Guid userRoleId)
        {
            await _userCommands.DeleteUserRole(userRoleId);
        }

        public async Task<IUserWithAuth> GetUserFromLogin(string subject, string issuer, string name)
        {
            var userInfo = await _userQueries.GetUserFromLogin(subject, issuer);
            if (userInfo == null)
            {
                return new NonAuthorizedUser(new UserInfo
                {
                    Name = name,
                    Status = UserStatus.None,
                });
            }
            await EnsureRoles();
            var userRoles = await _userQueries.ListUserRolesForUser(userInfo.UserId.Value);

            var permissionClaims = userRoles.SelectMany(ur =>
                GetRoleById(ur.RoleId).Permissions.Select(p =>
                    new PermissionClaim
                    {
                        PermissionCode = p.PermissionCode,
                        Attributes = p.Attributes.Select(pa =>
                            new PermissionClaimAttrib
                            {
                                Name = pa.Name,
                                Value = ur.Attributes.SingleOrDefault(ura => ura.Name.Equals(pa.Name))?.Value
                            }).ToArray()
                    }));

            return new AuthorizedUser(userInfo, permissionClaims);
        }
    }
}
