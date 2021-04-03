using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserQueries
    {
        Task<IEnumerable<Role>> ListRoles();
        Task<UserInfo> GetUser(Guid userId);
        Task<UserInfo> GetUserFromLogin(string subject, string issuer);
        Task<IEnumerable<UserRole>> ListUserRolesForUser(Guid userId);
        Task<IEnumerable<Permission>> ListPermissions();
        Task<IEnumerable<UserInfo>> ListUsers();
        Task<IEnumerable<UserInfo>> ListUsersByOrganization(Guid organizationId);
    }
}