using FPT.Models;
using FPT.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IJobRepository : IRepository<Job>
    {
        public void Update(Job job);
        Task<PaginatedList<Job>> GetAllFilteredAsync(
           Expression<Func<Job, bool>>? filter = null,
           string? includeProperties = null,
           int? cityId = null,
           int? jobtypeId = null,
           string? keyword = null,
           int pageIndex = 1
        );

        Task RemoveRangeByEmployerIdAsync(string employerId);

    }
}
