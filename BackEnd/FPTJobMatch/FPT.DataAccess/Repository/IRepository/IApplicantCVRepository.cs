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
        public void Update(ApplicantCV applicantCV);
        public int CountCVs(Expression<Func<ApplicantCV, bool>> filter);
    }
}
