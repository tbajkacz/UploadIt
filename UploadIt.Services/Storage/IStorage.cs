using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UploadIt.Services.Storage
{
    public interface IStorage
    {
        Task<bool> StoreFile(IFormFile file, int userId);
    }
}
