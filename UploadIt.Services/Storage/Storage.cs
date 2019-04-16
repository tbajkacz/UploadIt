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

        public async Task<bool> StoreFileAsync(IFormFile file, string directory)
        {
            //TODO validate user file name, look for \ etc
            var dir = GetAbsoluteDirectory(directory);
            EnsureDirectoryExists(dir);

            var path = Path.Combine(dir, file.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return true;
        }

        public async Task<byte[]> RetrieveFileAsync(string fileName, string directory)
        {
            //TODO validate filename/dir

            if (fileName == null || directory == null)
            {
                return null;
            }

            var dir = GetAbsoluteDirectory(directory);
            EnsureDirectoryExists(dir);

            var path = Path.Combine(dir, fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var memStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memStream);
                return memStream.GetBuffer();
            }
            
        }

        public string[] GetFileList(string directory)
        {
            var dir = GetAbsoluteDirectory(directory);
            EnsureDirectoryExists(dir);

            var filesFullPath = Directory.GetFiles(dir);

            return filesFullPath.Select(f => Path.GetFileName(f)).ToArray<String>();
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
