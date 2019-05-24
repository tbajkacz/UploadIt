using System.Threading.Tasks;
using UploadIt.Dto.Account;
using UploadIt.Model.Account;

namespace UploadIt.Services.Account
{
    /// <summary>
    /// Defines basic methods to use when handling user authentication
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Validates user credentials passed in <see cref="userParams"/> and returns the found user, null if not found
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        User Authenticate(UserLoginParams userParams);

        /// <summary>
        /// Tries adding the user passed in <see cref="userParams"/>, null if adding was not possible
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        Task<User> AddUserAsync(UserRegisterParams userParams);

        /// <summary>
        /// Deletes the user with the specified <see cref="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteUser(int id);

        /// <summary>
        /// Gets the user with the specified <see cref="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Determines if the passed username or email are already connected to an existing account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool UsernameOrEmailTaken(string username, string email);
    }
}
