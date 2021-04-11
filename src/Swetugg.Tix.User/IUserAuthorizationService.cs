using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserAuthorizationService
    {
        Task AddUserRoleById(Guid userId, Guid roleId, IEnumerable<UserRoleAttribute> attributes);
        Task AddUserRoleByName(Guid userId, string roleName, IEnumerable<UserRoleAttribute> attributes);
        Task<IUserWithAuth> GetUserFromLogin(string subject, string issuer, string name);
        Task RemoveUserRole(Guid userId, Guid userRoleId);
    }
}