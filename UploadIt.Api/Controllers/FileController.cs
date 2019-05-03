using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UploadIt.Data.Db.Account;
using UploadIt.Services.Helpers;
using UploadIt.Services.Security;
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
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITempTokenValidator _tokenValidator;
        private readonly IConfiguration _cfg;

        public FileController(IStorage storage,
                              AccountDbContext accDb,
                              ITokenGenerator tokenGenerator,
                              ITempTokenValidator tokenValidator,
                              IConfiguration cfg)
        {
            _storage = storage;
            _accDb = accDb;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;
            _cfg = cfg;
        }

        [HttpPost]
        [Route("UploadFile")]
        //max size is around 25 MB at most because of IIS limitations - due to change later on
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var userId = User.Identity.Name;

            if (file == null)
            {
                return BadRequest("No file uploaded");
            }

            await _storage.StoreFileAsync(file, userId);

            return Ok("File stored on the drive");
        }

        [HttpGet]
        [Route("GetDownloadToken")]
        public IActionResult GetDownloadToken()
        {
            var userId = User.Identity.Name;

            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, userId),
            };

            var downloadToken = _tokenGenerator
                .GenerateToken(
                _cfg.GetValue<string>("AppSettings:Secret"), claims, 0.5);

            return Ok(downloadToken.Token);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Download")]
        public async Task<IActionResult> DownloadFile([FromQuery]string token, [FromQuery]string fileName)
        {
            var claimsPrincipal = _tokenValidator.Validate(token, _cfg.GetValue<string>("AppSettings:Secret"));
            if (claimsPrincipal == null)
            {
                return BadRequest("Invalid token");
            }

            var userIdString = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid token");
            }
            
            ContentDisposition contentDisposition = new ContentDisposition
            {
                FileName = fileName,
                //inline true tries to display the file in the browser
                Inline = false
            };
            Response.Headers.Add("content-disposition", contentDisposition.ToString());
            var file = await _storage.RetrieveFileAsync(fileName, userIdString);
            if (file == null)
            {
                return BadRequest("File does not exist");
            }
            return File(file, FileContentType.Get(fileName));
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult DeleteFile([FromForm] string fileName)
        {
            if (fileName == null)
            {
                return BadRequest("Invalid query parameter");
            }

            var claims = User.Claims;

            if (claims == null)
            {
                return BadRequest("Invalid token");
            }

            var userIdString = claims
                .SingleOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (!_storage.DeleteFile(fileName, userIdString))
            {
                return BadRequest("Couldn't delete specified file");
            }

            return Ok("File successfully deleted");
        }

        [HttpGet]
        [Route("GetFiles")]
        public IActionResult GetFileList()
        {
            var userIdString = User.Identity.Name;

            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid user id");
            }

            var dtos = _storage.GetFileList(userId.ToString());

            return Ok(dtos);
        }
        
    }
}
