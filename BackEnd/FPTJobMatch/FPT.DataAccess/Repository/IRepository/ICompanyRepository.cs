using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public void Update(Company company);
        Task<bool> IsExistAsync(string companyName);
        Task RemoveByEmployerIdAsync(string employerId);
    }
}
