using System.Linq;
using System.Threading.Tasks;
using UploadIt.Data.Db.Account;
using UploadIt.Model.Account;

namespace UploadIt.Data.Repositories.Account
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _accountDb;

        public UserRepository(AccountDbContext accountDb)
        {
            _accountDb = accountDb;
        }

        public User GetByUserName(string userName)
        {
            return _accountDb.Users.SingleOrDefault(u =>
                u.UserName == userName);
        }

        public User GetByEmail(string email)
        {
            return _accountDb.Users.SingleOrDefault(u =>
                u.Email == email);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _accountDb.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _accountDb.Users.AddAsync(user);
            await _accountDb.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            _accountDb.Users.Remove(user);
            await _accountDb.SaveChangesAsync();
        }
    }
}
