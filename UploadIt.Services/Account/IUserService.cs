using System.Threading.Tasks;
using UploadIt.Dto.Account;
using UploadIt.Model.Account;

namespace UploadIt.Services.Account
{
    public interface IUserService
    {
        User Authenticate(UserLoginParams userParams);

        Task<User> AddUserAsync(UserRegisterParams userParams);

        Task DeleteUser(int id);

        Task<User> GetUserByIdAsync(int id);
    }
}
