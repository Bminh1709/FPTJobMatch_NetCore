using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IJobSeekerDetailRepository : IRepository<JobSeekerDetail>
    {
        public void Update(JobSeekerDetail jobSeekerDetail);
        Task RemoveByUserIdAsync(string userId);
    }
}
