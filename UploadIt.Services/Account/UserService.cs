using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Data.Repositories.Account;
using UploadIt.Dto.Account;
using UploadIt.Model.Account;

namespace UploadIt.Services.Account
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Authenticate(UserLoginParams userParams)
        {
            CheckArgs(userParams.UserName, userParams.Password);

            User user = _userRepository.GetByUserName(userParams.UserName);

            if (user == null || !VerifyPassword(userParams.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<User> AddUserAsync(UserRegisterParams userParams)
        {
            CheckArgs(userParams.UserName, userParams.Password, userParams.Email);

            if (UsernameOrEmailTaken(userParams.UserName, userParams.Email))
            {
                return null;
            }
            HashPassword(userParams.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new User
            {
                Email = userParams.Email,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                UserName = userParams.UserName,
                Role = UserRoles.Default
            };
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteAsync(id);
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
            return await _userRepository.GetByIdAsync(id);
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

        public bool UsernameOrEmailTaken(string username, string email)
        {
            return _userRepository.GetByUserName(username) != null &&
                   _userRepository.GetByEmail(email) != null;
        }
    }
}
