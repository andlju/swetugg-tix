using Dapper;
using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public class UserQueries : IUserQueries
    {
        private readonly string _connectionString;

        public UserQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserInfo> GetUser(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var userInfo = (await conn.QueryFirstOrDefaultAsync<UserInfo>(
                    "SELECT u.UserId, u.Name, u.Status " +
                    "FROM [Access].[User] u " +
                    "WHERE u.UserId = @UserId AND u.Status <> @DeletedStatus",
                    new { UserId = userId, DeletedStatus = UserStatus.Deleted }));

                return userInfo;
            }
        }

        public async Task<UserInfo> GetUserFromLogin(string subject, string issuer)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var userInfo = (await conn.QueryFirstOrDefaultAsync<UserInfo>(
                    "SELECT u.UserId, u.Name, u.Status " +
                    "FROM [Access].[User] u JOIN [Access].[UserLogin] ul ON ul.UserId = u.UserId JOIN [Access].[Issuer] i ON i.IssuerId = ul.IssuerId " +
                    "WHERE ul.Subject = @Subject AND i.IssuerIdentifier = @Issuer AND u.Status <> @DeletedStatus",
                    new { Subject = subject, Issuer = issuer, DeletedStatus = UserStatus.Deleted }));

                return userInfo;
            }
        }

        public async Task<IEnumerable<UserRole>> ListUserRolesForUser(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var userRoleLookup = new Dictionary<Guid, UserRole>();
                var userRoles = await conn.QueryAsync<UserRole, UserRoleAttribute, UserRole>(new CommandDefinition(
                    "SELECT ur.UserRoleId, r.RoleId, r.Name as RoleName, ura.UserRoleAttributeId, ura.Attribute, ura.Name " +
                    "FROM [Access].[UserRole] ur JOIN [Access].[Role] r ON ur.RoleId = r.RoleId " +
                    "JOIN [Access].[UserRoleAttribute] ura ON ur.UserRoleId = ura.UserRoleId " +
                    "WHERE ur.UserId = @UserId ", new { userId }), (ur, ura) =>
                    {
                        if (!userRoleLookup.TryGetValue(ur.UserRoleId, out var userRole))
                            userRoleLookup.Add(ur.UserRoleId, userRole = ur);
                        if (userRole.UserRoleAttributes == null)
                            userRole.UserRoleAttributes = new List<UserRoleAttribute>();
                        userRole.UserRoleAttributes.Add(ura);
                        return userRole;
                    }, "UserRoleId");

                return userRoleLookup.Values.ToArray();
            }
        }

        public async Task<IEnumerable<Role>> ListRoles()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var roleLookup = new Dictionary<Guid, Role>();
                var roles = await conn.QueryAsync<Role, Permission, PermissionAttribute, Role>(
                    "SELECT r.RoleId, r.Name, r.Description, pa.PermissionCode, pa.Attribute as Name " +
                    "FROM [Access].[Role] r JOIN [Access].[RolePermission] rp ON r.RoleId = rp.RoleId " +
                    "JOIN [Access].[PermissionAttribute] pa ON rp.PermissionCode = pa.PermissionCode ", (r, p, pa) => {
                        if (!roleLookup.TryGetValue(r.RoleId, out var role))
                            roleLookup.Add(r.RoleId, role = r);
                        if (role.Permissions == null)
                            role.Permissions = new List<Permission>();
                        var permission = role.Permissions.FirstOrDefault(rp => rp.PermissionCode == p.PermissionCode);
                        if (permission == null)
                            role.Permissions.Add(permission = p);
                        if (permission.Attributes == null)
                            permission.Attributes = new List<PermissionAttribute>();
                        permission.Attributes.Add(pa);
                        
                        return role;
                    }, splitOn: "PermissionCode,Name");

                return roleLookup.Values.ToArray();
            }
        }

        public async Task<IEnumerable<Permission>> ListPermissions()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var permissionLookup = new Dictionary<string, Permission>();
                var roles = await conn.QueryAsync<Permission, PermissionAttribute, Permission>(
                    "SELECT p.PermissionCode, p.Description, pa.Attribute as Name " +
                    "FROM [Access].[Permission] p JOIN [Access].[PermissionAttribute] pa ON p.PermissionCode = pa.PermissionCode ", (p, pa) => {
                        if (!permissionLookup.TryGetValue(p.PermissionCode, out var permission))
                            permissionLookup.Add(p.PermissionCode, permission = p);
                        if (permission.Attributes == null)
                            permission.Attributes = new List<PermissionAttribute>();
                        permission.Attributes.Add(pa);

                        return permission;
                    }, splitOn: "Name");

                return permissionLookup.Values.ToArray();
            }
        }

    }
}
