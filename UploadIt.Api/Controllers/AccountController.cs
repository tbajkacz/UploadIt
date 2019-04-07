using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UploadIt.Data.Db.Account;
using UploadIt.Data.Models.Account;
using UploadIt.Services.Account;

namespace UploadIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AccountController(IUserService userService,
                                 IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [AllowAnonymous]
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromForm]LoginForm form)
        {
            User user = _userService.Authenticate(form.UserName, form.Password);
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("AppSettings:Secret")));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var tokenString = jwtTokenHandler.WriteToken(token);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                tokenString
            });
        }

        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]RegisterForm form)
        {
            try
            {
                User user = await _userService.AddUserAsync(form.UserName,
                    form.Password,
                    form.Email);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return BadRequest("Invalid form data");
            }

            return Ok("Account created");
        }

        [Route("Delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user id");
            }

            try
            {
                await _userService.DeleteUser(userId);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
            return Ok($"User with id {userId} successfully deleted");
        }

        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user id");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }
    }
}
