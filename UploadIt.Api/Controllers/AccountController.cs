using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UploadIt.Dto.Account;
using UploadIt.Model.Account;
using UploadIt.Services.Account;
using UploadIt.Services.Security;

namespace UploadIt.Api.Controllers
{
    /// <summary>
    /// Controller used to issue jwt tokens and manage user accounts
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly ITokenGenerator _tokenGenerator;

        public AccountController(IUserService userService,
                                 IConfiguration config,
                                 ITokenGenerator tokenGenerator)
        {
            _userService = userService;
            _config = config;
            this._tokenGenerator = tokenGenerator;
        }

        /// <summary>
        /// Authenticates the user based on the data provided in <paramref name="form"/> and returns user info and a jwt token if authentication succeeds
        /// </summary>
        /// <param name="form"></param>
        /// <returns>Object containing: string userName, string email, string token, DateTime validTo</returns>
        [AllowAnonymous]
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromForm]UserLoginParams form)
        {
            User user = null;
            try
            {
                user = _userService.Authenticate(form);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };


            var tokenInfo = _tokenGenerator.GenerateJwtToken(_config.GetValue<string>("AppSettings:Secret"), claims, 15);

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenInfo.Token,
                ValidTo = tokenInfo.ValidTo
            });
        }

        /// <summary>
        /// Registers a user using the data provided in <paramref name="form"/>
        /// </summary>
        /// <param name="form"></param>
        /// <returns>String which describes if the operation succeeded</returns>
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]UserRegisterParams form)
        {
            User user;
            try
            {
                user = await _userService.AddUserAsync(form);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return BadRequest("Invalid form data");
            }

            if (user == null)
            {
                return BadRequest(
                    "User with the provided username or email already exists");
            }

            return Ok("Account created");
        }

        /// <summary>
        /// Deletes the user bound to the jwtToken which was used for authorization
        /// </summary>
        /// <returns>String which describes if the operation succeeded</returns>
        [Route("Delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userIdString = User.Identity.Name;

            if (!int.TryParse(userIdString, out int userId))
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
    }
}
