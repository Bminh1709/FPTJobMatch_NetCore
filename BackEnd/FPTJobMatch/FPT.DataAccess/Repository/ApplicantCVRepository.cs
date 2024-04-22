using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository
{
    public class ApplicantCVRepository : Repository<ApplicantCV>, IApplicantCVRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicantCVRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public async Task<int> CountCVsAsync(Expression<Func<ApplicantCV, bool>> filter)
        {
            return await _db.ApplicantCVs.CountAsync(filter);
        }

        public async Task<IEnumerable<ApplicantCV>> GetAllJobFilteredAsync(int jobId, string? status = null, string? sortType = null)
        {
            IQueryable<ApplicantCV> query = _db.ApplicantCVs.Include(a => a.JobSeeker).Where(a => a.JobId == jobId);

            if (status != "All" && !string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.CVStatus == status);
            }

            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType == "NewestFirst")
                {
                    query = query.OrderByDescending(a => a.DateSubmitted);
                }
                else if (sortType == "OldestFirst")
                {
                    query = query.OrderBy(a => a.DateSubmitted);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsSubmittedLast30DaysAsync(int jobId, string userId)
        {
            var applicantCV = await _db.ApplicantCVs.FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == userId);
            if (applicantCV != null)
            {
                int gapDays = (DateTime.UtcNow - applicantCV.DateSubmitted).Days;
                return gapDays < 30;
            }
            else
            {
                return false;
            }
        }

        public void Update(ApplicantCV applicantCV)
        {
            _db.ApplicantCVs.Update(applicantCV);
        }
    }

}
