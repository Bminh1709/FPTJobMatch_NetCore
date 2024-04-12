using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;

namespace FPT.DataAccess.Repository
{
    public class JobTypeRepository : Repository<JobType>, IJobTypeRepository
    {
        private ApplicationDbContext _db;
        public JobTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(JobType jobType)
        {
            _db.JobTypes.Update(jobType);
        }
    }
}
