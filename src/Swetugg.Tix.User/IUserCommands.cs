using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserCommands
    {
        Task<Guid> AssignUserToRole(Guid userId, Guid roleId, IEnumerable<PermissionClaimAttrib> permissionAttributes);
        Task<Guid> CreateUser(UserInfo info);
        Task DeleteUserRole(Guid userRoleId);
        Task SetUser(UserInfo info);
    }
}