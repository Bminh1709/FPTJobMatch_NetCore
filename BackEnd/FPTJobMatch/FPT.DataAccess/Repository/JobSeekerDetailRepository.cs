using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;

namespace FPT.DataAccess.Repository
{
    public class JobSeekerDetailRepository : Repository<JobSeekerDetail>, IJobSeekerDetailRepository
    {
        private ApplicationDbContext _db;
        public JobSeekerDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(JobSeekerDetail jobSeekerDetail)
        {
            _db.JobSeekerDetails.Update(jobSeekerDetail);
        }
    }
}
