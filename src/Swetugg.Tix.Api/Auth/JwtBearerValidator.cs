using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Swetugg.Tix.Api.Options;

namespace Swetugg.Tix.Api.Auth
{
    public class JwtBearerValidator
    {
        private ApiOptions _apiOptions;
        private ILogger _log;
        private const string scopeType = @"http://schemas.microsoft.com/identity/claims/scope";
        private ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private ClaimsPrincipal _claimsPrincipal;

        private string _wellKnownEndpoint = string.Empty;
        private string _tenantName = string.Empty;
        private string _policyName = string.Empty;
        private string _clientId = string.Empty;
        private string _requiredScope = "read-as-user";

        public JwtBearerValidator(IOptions<ApiOptions> options, ILoggerFactory loggerFactory)
        {
            _apiOptions = options.Value;
            _log = loggerFactory.CreateLogger<JwtBearerValidator>();

            _clientId = _apiOptions.AzureAdClientId;
            _tenantName = _apiOptions.AzureAdTenantName;
            _policyName = _apiOptions.AzureAdPolicyName;

            _wellKnownEndpoint = $"https://{_tenantName}.b2clogin.com/{_tenantName}.onmicrosoft.com/{_policyName}/v2.0/.well-known/openid-configuration";
            // _wellKnownEndpoint = $"{_instance}{_tenantId}/v2.0/.well-known/openid-configuration";
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            if (!authorizationHeader.Contains("Bearer"))
            {
                return null;
            }

            var accessToken = authorizationHeader.Substring("Bearer ".Length);

            var oidcWellknownEndpoints = await GetOIDCWellknownConfiguration();

            var tokenValidator = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = _clientId,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                ValidIssuer = oidcWellknownEndpoints.Issuer
            };

            try
            {
                SecurityToken securityToken;
                _claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);

                if (IsScopeValid(_requiredScope))
                {
                    return _claimsPrincipal;
                }

                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
            }
            return null;
        }

        public string GetPreferredUserName()
        {
            string preferredUsername = string.Empty;
            var preferred_username = _claimsPrincipal.Claims.FirstOrDefault(t => t.Type == "preferred_username");
            if (preferred_username != null)
            {
                preferredUsername = preferred_username.Value;
            }

            return preferredUsername;
        }

        private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfiguration()
        {
            _log.LogDebug($"Get OIDC well known endpoints {_wellKnownEndpoint}");
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                 _wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());

            return await _configurationManager.GetConfigurationAsync();
        }

        private bool IsScopeValid(string scopeName)
        {
            if (_claimsPrincipal == null)
            {
                _log.LogWarning($"Scope invalid {scopeName}");
                return false;
            }

            var scopeClaim = _claimsPrincipal.HasClaim(x => x.Type == scopeType)
                ? _claimsPrincipal.Claims.First(x => x.Type == scopeType).Value
                : string.Empty;

            if (string.IsNullOrEmpty(scopeClaim))
            {
                _log.LogWarning($"Scope invalid {scopeName}");
                return false;
            }

            if (!scopeClaim.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
            {
                _log.LogWarning($"Scope invalid {scopeName}");
                return false;
            }

            _log.LogDebug($"Scope valid {scopeName}");
            return true;
        }
    }
}