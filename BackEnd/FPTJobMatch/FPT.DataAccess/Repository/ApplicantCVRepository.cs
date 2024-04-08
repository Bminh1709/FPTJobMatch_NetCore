using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository
{
    public class ApplicantCVRepository : Repository<ApplicantCV>, IApplicantCVRepository
    {
        private ApplicationDbContext _db;
        public ApplicantCVRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicantCV applicantCV)
        {
            _db.ApplicantCVs.Update(applicantCV);
        }
    }
}
