using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploadIt.Model.Registration;

namespace UploadIt.Data.Db.Registration
{
    public class RegistrationDbContext : DbContext
    {
        public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options) : base(options)
        {
            
        }

        public DbSet<RegistrationRequest> Requests { get; set; }
    }
}
