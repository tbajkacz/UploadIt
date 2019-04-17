using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadIt.Data.Models.Security
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public DateTime ValidTo { get; set; }
    }
}
