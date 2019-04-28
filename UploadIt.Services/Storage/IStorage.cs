using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UploadIt.Dto.Files;

namespace UploadIt.Services.Storage
{
    public interface IStorage
    {
        Task<bool> StoreFileAsync(IFormFile file, string directory);

        Task<byte[]> RetrieveFileAsync(string fileName, string directory);

        IEnumerable<FileDto> GetFileList(string directory);
    }
}
