using System.Security.Claims;
using UploadIt.Model.Security;

namespace UploadIt.Services.Security
{
    public interface ITokenGenerator
    {
        TokenInfo GenerateToken(string secret, Claim[] claims, double minutesExpirationTime);
    }
}
