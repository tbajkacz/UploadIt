using System.Security.Claims;

namespace UploadIt.Services.Security
{
    public interface ITempTokenValidator
    {
        ClaimsPrincipal Validate(string token, string secret);
    }
}
