using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UploadIt.Dto.Files;

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
            if (AnyIsNull(file, directory))
            {
                return false;
            }
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

            if (AnyIsNull(fileName, directory))
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
                return memStream.ToArray();
            }
            
        }

        public bool DeleteFile(string fileName, string directory)
        {
            if (AnyIsNull(fileName, directory))
            {
                return false;
            }

            var dir = GetAbsoluteDirectory(directory);
            EnsureDirectoryExists(dir);

            var path = Path.Combine(dir, fileName);

            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);

            return true;
        }

        public IEnumerable<FileDto> GetFileList(string directory)
        {
            if (AnyIsNull(directory))
            {
                return null;
            }

            var dir = GetAbsoluteDirectory(directory);
            EnsureDirectoryExists(dir);

            var filesFullPath = Directory.GetFiles(dir);

            return filesFullPath
                .Select(p =>
                {
                    var fileInfo = new FileInfo(p);
                    return new FileDto{Name = fileInfo.Name, Size = fileInfo.Length};
                });
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


        private bool AnyIsNull(params object[] args) =>
            args.Any(a => a == null);
    }
}
