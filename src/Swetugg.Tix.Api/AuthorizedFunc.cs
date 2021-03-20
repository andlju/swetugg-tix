using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Api.Authorization;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api
{
    public class EmptyFuncParams
    {

    }

    public abstract class AuthorizedFunc<TFuncParams>
        where TFuncParams: class
    {
        private readonly IAuthManager _authManager;
        private readonly string[] _acceptedScopes;

        public IAuthManager AuthManager => _authManager;

        public string[] AcceptedScopes => _acceptedScopes;

        protected AuthorizedFunc(IAuthManager authManager, string[] acceptedScopes = null)
        {
            _authManager = authManager;
            _acceptedScopes = acceptedScopes ?? new[] { "access_as_user", "access_as_admin" };
        }

        protected async Task<IActionResult> Process(HttpRequest req, ILogger log, TFuncParams funcParams = null, bool allowUnauthorized = false)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var authenticationResponse = await _authManager.Authenticate(AcceptedScopes);
            if (authenticationResponse != null && !allowUnauthorized)
                return authenticationResponse;

            funcParams = await HandleUser(req, log, funcParams);

            return await HandleRequest(req, log, funcParams);
        }

        protected abstract Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, TFuncParams funcParams);

        protected virtual Task<TFuncParams> HandleUser(HttpRequest req, ILogger log, TFuncParams funcParams)
        {
            return Task.FromResult(funcParams);
        }
    }
}
