using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Data.Models.Security;

namespace UploadIt.Services.Security
{
    public class TokenGenerator : ITokenGenerator
    {
        public TokenInfo GenerateJwtToken(string secret, Claim[] claims, int minutesExpirationTime)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutesExpirationTime),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var tokenString = jwtTokenHandler.WriteToken(token);

            return new TokenInfo { Token = tokenString, ValidTo = token.ValidTo };
        }
    }
}
