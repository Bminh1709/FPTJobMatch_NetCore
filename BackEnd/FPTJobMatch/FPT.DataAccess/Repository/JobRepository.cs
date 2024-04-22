using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Job>> GetAllFilteredAsync(Expression<Func<Job, bool>>? filter = null, string? includeProperties = null, int? cityId = null, int? jobtypeId = null, string? keyword = null)
        {
            IQueryable<Job> query = _db.Jobs;

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related entities if provided
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply additional filtering based on cityId, jobtypeId, and keyword
            if (cityId.HasValue)
            {
                query = query.Where(j => j.Company.CityId == cityId);
            }

            if (jobtypeId.HasValue)
            {
                query = query.Where(j => j.JobTypeId == jobtypeId);
            }

            // Fetch the data asynchronously
            var jobs = await query.ToListAsync();

            // Apply keyword filter on the client side
            if (!string.IsNullOrEmpty(keyword))
            {
                jobs = jobs.Where(j => j.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Execute the query asynchronously and return the result
            return jobs;
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
