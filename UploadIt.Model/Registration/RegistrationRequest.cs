using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Dto.Account;

namespace UploadIt.Model.Registration
{
    public class RegistrationRequest : UserRegisterParams
    {
        public int Id { get; set; }
    }
}
