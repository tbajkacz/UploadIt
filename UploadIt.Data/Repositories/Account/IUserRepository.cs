using System.Threading.Tasks;
using UploadIt.Model.Account;

namespace UploadIt.Data.Repositories.Account
{
    public interface IUserRepository
    {
        User GetByUserName(string userName);

        User GetByEmail(string email);

        Task<User> GetByIdAsync(int id);

        Task AddAsync(User user);

        Task DeleteAsync(int id);
    }
}
