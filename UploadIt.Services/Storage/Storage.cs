using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UploadIt.Services.Storage
{
    public class Storage : IStorage
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _cfg;

        public Storage(IHostingEnvironment env, IConfiguration cfg)
        {
            _env = env;
            _cfg = cfg;
        }

        public async Task<bool> StoreFile(IFormFile file, int userId)
        {
            //TODO validate user file name, look for \ etc
            var dir = GetAbsoluteDirectory(userId.ToString());
            EnsureDirectoryExists(dir);

            var path = Path.Combine(dir, file.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return true;
        }

        private void EnsureDirectoryExists(string absoluteDirPath)
        {
            if (!Directory.Exists(absoluteDirPath))
            {
                Directory.CreateDirectory(absoluteDirPath);
            }
        }

        private string GetAbsoluteDirectory(string dir) =>
            Path.Combine(_env.ContentRootPath, _cfg.GetValue<string>("AppSettings:UserStorageDirectoryName"), dir);
    }
}
