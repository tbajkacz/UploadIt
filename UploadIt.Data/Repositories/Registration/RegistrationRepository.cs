using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Data.Db.Registration;
using UploadIt.Model.Registration;

namespace UploadIt.Data.Repositories.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly RegistrationDbContext _dbContext;

        public RegistrationRepository(RegistrationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddAsync(RegistrationRequest request)
        {
            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<RegistrationRequest> GetAll()
        {
            return _dbContext.Requests;
        }

        public async Task<RegistrationRequest> GetByIdAsync(int id)
        {
            return await _dbContext.Requests.FindAsync(id);
        }

        public async Task RemoveByIdAsync(int id)
        {
            var entity = await _dbContext.Requests.FindAsync(id);
            _dbContext.Requests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
