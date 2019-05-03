using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UploadIt.Services.Security
{
    public class JwtTempTokenValidator : ITempTokenValidator
    {
        public JwtTempTokenValidator()
        {
        }

        public ClaimsPrincipal Validate(string token, string secret)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return null;
            }

            try
            {
                var claimsPrincipal = handler.ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        //default ClockSkew is 5 minutes and it causes expired tokens to be successfully validated
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken jwtToken);
                return claimsPrincipal;
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }
            catch (SecurityTokenNoExpirationException)
            {
                return null;
            }
        }
    }
}
