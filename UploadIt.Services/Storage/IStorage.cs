using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UploadIt.Services.Storage
{
    public interface IStorage
    {
        Task<bool> StoreFileAsync(IFormFile file, string directory);

        Task<byte[]> RetrieveFileAsync(string fileName, string directory);

        string[] GetFileList(string directory);
    }
}
