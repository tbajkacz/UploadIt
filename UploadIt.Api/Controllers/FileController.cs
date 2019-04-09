﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadIt.Data.Db.Account;
using UploadIt.Data.Models.Account;
using UploadIt.Services.Helpers;
using UploadIt.Services.Storage;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UploadIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IStorage _storage;
        private readonly AccountDbContext _accDb;

        public FileController(IStorage storage, AccountDbContext accDb)
        {
            _storage = storage;
            _accDb = accDb;
        }

        [HttpPost]
        [Route("UploadFile")]
        //max size is around 25 MB at most because of IIS limitations - due to change later on
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user id");
            }

            if (file == null)
            {
                return BadRequest("No file uploaded");
            }

            await _storage.StoreFile(file, userId.ToString());

            return Ok("File stored on the drive");
        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(
            [FromForm] string fileName)
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user id");
            }
            
            var file = await _storage.RetrieveFile(fileName, userId.ToString());
            return File(file, FileContentType.Get(fileName));
        }

        [Route("Test")]
        public IActionResult Test(string msg)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return Ok("Hello " + claim?.Value);
        }

        [AllowAnonymous]
        [Route("Test2")]
        public IActionResult Test2([FromForm] IFormFile file)
        {
            return Ok();
        }
    }
}
