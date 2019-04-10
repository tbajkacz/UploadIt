using Microsoft.EntityFrameworkCore;
using UploadIt.Data.Models.Account;

namespace UploadIt.Data.Db.Account
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
