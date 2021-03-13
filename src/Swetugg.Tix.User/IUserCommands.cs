using Swetugg.Tix.User.Contract;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserCommands
    {
        Task<Guid> CreateUser(UserInfo info);
        Task SetUser(UserInfo info);
    }
}