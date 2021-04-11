using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserCommands
    {
        Task<Guid> AddUserRole(Guid userId, Guid roleId, IEnumerable<UserRoleAttribute> attributes);
        Task<Guid> CreateUser(UserInfo info);
        Task DeleteUserRole(Guid userRoleId);
        Task SetUser(UserInfo info);
    }
}