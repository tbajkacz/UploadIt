using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadIt.Services.Storage;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UploadIt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IStorage _storage;

        public FileController(IStorage storage)
        {
            _storage = storage;
        }
        [HttpPost]
        [Route("UploadFile")]
        //size is around 25 MB at most because of IIS limitations - due to change later on
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("No file uploaded");
            }

            await _storage.StoreFile(file, "");

            return Ok("File stored on the drive");
        }

        [Route("Test")]
        public IActionResult Test(string msg)
        {
            return Ok("Test");
        }
    }
}
