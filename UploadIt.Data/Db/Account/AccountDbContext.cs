using Microsoft.EntityFrameworkCore;
using UploadIt.Model.Account;

namespace UploadIt.Data.Db.Account
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
