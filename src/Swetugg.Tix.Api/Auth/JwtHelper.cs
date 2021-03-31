using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Swetugg.Tix.Api.Authorization
{
    public class JwtHelper : IJwtHelper
    {
        private readonly SymmetricSecurityKey _secureKey;

        public JwtHelper(string secureKey)
        {
            _secureKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secureKey));
        }

        public string GenerateActionJwtToken(string action, DateTime expiresUtc, IDictionary<string, string> claims)
        {
            var allClaims = claims.Select(c => new Claim(c.Key, c.Value)).ToList();
            allClaims.Add(new Claim("Action", action));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(allClaims),
                Expires = expiresUtc,
                
                SigningCredentials = new SigningCredentials(_secureKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IDictionary<string, string> ValidateActionToken(string token, string action)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = _secureKey
                }, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                    return null;

                return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
