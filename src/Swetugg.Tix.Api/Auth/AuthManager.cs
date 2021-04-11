
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.User;
using Swetugg.Tix.User.Contract;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Authorization
{


    public class AuthManager : IAuthManager
    {
        private readonly IUserAuthorizationService _userService;
        private readonly HttpRequest _currentRequest;

        public AuthManager(IHttpContextAccessor httpContextAccessor, IUserAuthorizationService userService)
        {
            _currentRequest = httpContextAccessor.HttpContext.Request;
            _userService = userService;
        }

        public async Task<IActionResult> Authenticate(string[] acceptedScopes)
        {
            var (authenticationStatus, authenticationResponse) = await _currentRequest.HttpContext.AuthenticateAzureFunctionAsync();
            if (!authenticationStatus) return authenticationResponse;
            try
            {
                _currentRequest.HttpContext.VerifyUserHasAnyAcceptedScope(acceptedScopes);
            }
            catch (HttpRequestException ex)
            {
                return new UnauthorizedObjectResult(ex.Message);
            }
            return null;
        }

        public async Task<IUserWithAuth> GetAuthorizedUser()
        {
            var identity = _currentRequest.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            if (identity is null || !identity.IsAuthenticated)
                return null;

            var subject = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var issuer = identity.FindFirst("iss").Value;
            string name = identity.IsAuthenticated ? identity.Name : null;

            return await _userService.GetUserFromLogin(subject, issuer, name);
        }
    }
}
