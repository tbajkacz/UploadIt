using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UploadIt.Services.Storage
{
    public class Storage : IStorage
    {
        public async Task<bool> StoreFile(IFormFile file, string dest)
        {
            using (var fileStream = new FileStream(Path.Combine(dest + file.FileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return true;
        }
    }
}
