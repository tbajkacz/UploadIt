using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Data.Models.Account;

namespace UploadIt.Services.Account
{
    public interface IUserService
    {
        User Authenticate(string userName, string password);

        Task<User> AddUserAsync(string userName, string password, string email);

        Task DeleteUser(int id);

        Task<User> GetUserByIdAsync(int id);
    }
}
