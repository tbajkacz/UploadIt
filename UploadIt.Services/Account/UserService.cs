using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploadIt.Data.Db.Account;
using UploadIt.Data.Models.Account;

namespace UploadIt.Services.Account
{
    public class UserService : IUserService
    {
        private readonly AccountDbContext _accountDb;

        public UserService(AccountDbContext accountDb)
        {
            _accountDb = accountDb;
        }

        public User Authenticate(string userName, string password)
        {
            CheckArgs(userName, password);

            User user =
                _accountDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null || !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<User> AddUserAsync(string userName, string password, string email)
        {
            CheckArgs(userName, password, email);

            if (_accountDb.Users.Any(u => u.UserName == userName && u.Email == email))
            {
                return null;
            }
            HashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new User
            {
                Email = email,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                UserName = userName
            };
            await _accountDb.Users.AddAsync(user);
            await _accountDb.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("Invalid id parameter");
            }
            _accountDb.Remove(user);
            await _accountDb.SaveChangesAsync();
        }

        private static void HashPassword(string password, out byte[] hash, out byte[] salt)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null, empty or whitespace");
            }

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                salt = hmac.Key;
            }
        }

        private static bool VerifyPassword(string password,
            byte[] storedPasswordHash, byte[] storedPasswordSalt)
        {
            if (storedPasswordHash == null || storedPasswordSalt == null || password == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty or whitespace");
            }

            if (storedPasswordHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).");
            }

            if (storedPasswordSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).");
            }

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                hmac.Key = storedPasswordSalt;
                var passwordHash =
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != storedPasswordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _accountDb.Users.FindAsync(id);
        }

        /// <summary>
        /// Throws an exception if any of the <see cref="args"/> is null, whitespace or empty
        /// </summary>
        /// <param name="args"></param>
        private static void CheckArgs(params string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    throw new ArgumentNullException();
                }

                if (string.IsNullOrEmpty(arg) || string.IsNullOrWhiteSpace(arg))
                {
                    throw new ArgumentException("String argument cannot be whitespace or empty");
                }
            }
        }
    }
}
