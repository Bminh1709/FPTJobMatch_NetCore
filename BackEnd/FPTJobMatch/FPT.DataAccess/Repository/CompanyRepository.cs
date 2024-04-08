using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;

namespace FPT.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public bool IsExist(string companyName)
        {
            return _db.Companies.Any(c => c.Name == companyName);
        }

        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
