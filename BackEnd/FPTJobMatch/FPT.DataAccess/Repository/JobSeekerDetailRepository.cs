using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;

namespace FPT.DataAccess.Repository
{
    public class JobSeekerDetailRepository : Repository<JobSeekerDetail>, IJobSeekerDetailRepository
    {
        private ApplicationDbContext _db;
        public JobSeekerDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task RemoveByUserIdAsync(string userId)
        {
            var detail = await _db.JobSeekerDetails.FirstOrDefaultAsync(d => d.JobSeekerId == userId);
            if (detail != null)
            {
                _db.JobSeekerDetails.Remove(detail);
            }
        }

        public void Update(JobSeekerDetail jobSeekerDetail)
        {
            _db.JobSeekerDetails.Update(jobSeekerDetail);
        }
    }
}
