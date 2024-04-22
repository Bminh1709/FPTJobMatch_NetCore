using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IApplicantCVRepository : IRepository<ApplicantCV>
    {
        void Update(ApplicantCV applicantCV);

        Task<int> CountCVsAsync(Expression<Func<ApplicantCV, bool>> filter);

        Task<bool> IsSubmittedLast30DaysAsync(int jobId, string userId);
        Task<IEnumerable<ApplicantCV>> GetAllJobFilteredAsync(int jobId, string? status = null, string? sortType = null);
    }
}
