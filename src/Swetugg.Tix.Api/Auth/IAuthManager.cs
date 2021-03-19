using Microsoft.AspNetCore.Mvc;
using Swetugg.Tix.User.Contract;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Authorization
{
    public interface IAuthManager
    {
        Task<IActionResult> Authenticate(string[] acceptedScopes);
        Task<UserInfo> GetAuthenticatedUser();
    }
}