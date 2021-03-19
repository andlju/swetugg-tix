using Swetugg.Tix.User.Contract;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public interface IUserQueries
    {
        Task<UserInfo> GetUser(Guid userId);
        Task<UserInfo> GetUserFromLogin(string subject, string issuer);
    }
}