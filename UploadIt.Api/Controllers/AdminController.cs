using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UploadIt.Data.Repositories.Registration;
using UploadIt.Dto.Account;
using UploadIt.Dto.Registration;
using UploadIt.Model.Account;
using UploadIt.Services.Account;

namespace UploadIt.Api.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRegistrationRepository _registrationRepository;

        public AdminController(IUserService userService,
                               IRegistrationRepository registrationRepository)
        {
            this._userService = userService;
            this._registrationRepository = registrationRepository;
        }

        /// <summary>
        /// Registers a user using the data provided in <paramref name="form"/> - only available to admins, new users should use the Register endpoint
        /// </summary>
        /// <param name="form"></param>
        /// <returns>String which describes if the operation succeeded</returns>
        [Route("AcceptRegisterRequest")]
        [HttpPost]
        public async Task<IActionResult> RegisterConfirmation([FromForm]int id)
        {
            User user;
            try
            {
                user = await _userService.AddUserAsync(await _registrationRepository.GetByIdAsync(id));
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

        [Route("DeleteRegisterRequest")]
        public async Task<IActionResult> DeleteEntry([FromForm]int id)
        {
            await _registrationRepository.RemoveByIdAsync(id);

            return Ok($"Successfully deleted request with id {id}");
        }

        [Route("GetRegistrationRequests")]
        [HttpGet]
        public IActionResult GetRegistrationRequests()
        {
            var data = _registrationRepository
                .GetAll()
                .Select(r => new RegistrationRequestDto
                    {Email = r.Email, UserName = r.UserName, Id = r.Id});
            return Ok(data);
        }
    }
}
