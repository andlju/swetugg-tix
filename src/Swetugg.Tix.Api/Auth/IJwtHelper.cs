using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Api.Authorization
{
    public interface IJwtHelper
    {
        string GenerateActionJwtToken(string action, DateTime expiresUtc, IDictionary<string, string> claims);
        IDictionary<string, string> ValidateActionToken(string token, string action);
    }
}