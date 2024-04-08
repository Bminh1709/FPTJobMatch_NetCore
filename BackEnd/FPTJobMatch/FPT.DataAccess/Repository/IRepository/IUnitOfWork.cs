using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        IJobSeekerDetailRepository JobSeekerDetail { get; }
        IStatusRepository AccountStatus { get; }
        ICompanyRepository Company { get; }
        ICategoryRepository Category { get; }
        IJobRepository Job { get; }
        ICityRepository City { get; }
        IApplicantCVRepository ApplicantCV { get; }
        void Save();
    }
}
