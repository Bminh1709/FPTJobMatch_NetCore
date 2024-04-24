using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Utility.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace FPT.DataAccess.Repository
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        private ApplicationDbContext _db;
        public JobRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Job>> GetAllFilteredAsync(Expression<Func<Job, bool>>? filter, string? includeProperties, int? cityId, int? jobtypeId, string? keyword, int pageIndex)
        {
            IQueryable<Job> jobs = _db.Jobs;

            // Apply filter if provided
            if (filter != null)
            {
                jobs = jobs.Where(filter);
            }

            // Include related entities if provided
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    jobs = jobs.Include(includeProperty);
                }
            }

            // Apply additional filtering based on cityId, jobtypeId, and keyword
            if (cityId.HasValue)
            {
                jobs = jobs.Where(j => j.Company.CityId == cityId);
            }

            if (jobtypeId.HasValue)
            {
                jobs = jobs.Where(j => j.JobTypeId == jobtypeId);
            }

            // Apply keyword filter on the client side
            if (!string.IsNullOrEmpty(keyword))
            {
                jobs = jobs.Where(j => j.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            // Set Page Size
            int pageSize = 5;

            // Use PaginatedList<T>.CreateAsync to create paginated results
            return await PaginatedList<Job>.CreateAsync(jobs, pageIndex, pageSize);
        }

        public async Task RemoveRangeByEmployerIdAsync(string employerId)
        {
            var jobs = await _db.Jobs.Where(job => job.EmployerId == employerId).ToListAsync();
            if (jobs != null)
            {
                _db.Jobs.RemoveRange(jobs);
            }
        }

        public void Update(Job job)
        {
            _db.Jobs.Update(job);
        }
    }
}
