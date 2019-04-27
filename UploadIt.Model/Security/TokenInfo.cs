using System;

namespace UploadIt.Model.Security
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public DateTime ValidTo { get; set; }
    }
}
