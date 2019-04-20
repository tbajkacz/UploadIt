using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Data.Models.Security;

namespace UploadIt.Services.Security
{
    public interface ITokenGenerator
    {
        TokenInfo GenerateJwtToken(string secret, Claim[] claims, double minutesExpirationTime);
    }
}
