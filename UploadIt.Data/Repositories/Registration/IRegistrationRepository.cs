using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadIt.Model.Registration;

namespace UploadIt.Data.Repositories.Registration
{
    public interface IRegistrationRepository
    {
        Task AddAsync(RegistrationRequest request);

        Task RemoveByIdAsync(int id);

        Task<RegistrationRequest> GetByIdAsync(int id);

        IEnumerable<RegistrationRequest> GetAll();
    }
}
