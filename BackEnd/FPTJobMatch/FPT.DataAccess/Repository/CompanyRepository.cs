using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;

namespace FPT.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> IsExistAsync(string companyName)
        {
            return await _db.Companies.AnyAsync(c => c.Name == companyName);
        }

        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
