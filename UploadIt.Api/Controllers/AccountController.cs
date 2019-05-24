using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UploadIt.Data.Repositories.Registration;
using UploadIt.Dto.Account;
using UploadIt.Model.Account;
using UploadIt.Model.Registration;
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
        private readonly IRegistrationRepository _registrationRepository;

        public AccountController(IUserService userService,
                                 IConfiguration config,
                                 ITokenGenerator tokenGenerator,
                                 IRegistrationRepository registrationRepository)
        {
            _userService = userService;
            _config = config;
            this._tokenGenerator = tokenGenerator;
            this._registrationRepository = registrationRepository;
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
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };


            var tokenInfo = _tokenGenerator.GenerateToken(_config.GetValue<string>("AppSettings:Secret"), claims, 15);

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenInfo.Token,
                ValidTo = tokenInfo.ValidTo
            });
        }

        

        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromForm] UserRegisterParams form)
        {
            if (_userService.UsernameOrEmailTaken(form.UserName, form.Email))
            {
                return BadRequest("Username or email already taken");
            }

            try
            {
                await _registrationRepository.AddAsync(new RegistrationRequest
                {
                    Email = form.Email,
                    Password = form.Password,
                    UserName = form.UserName
                });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return BadRequest("Invalid data provided");
            }

            return Ok("Account creation request sent");
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
