using FPT.Models;
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
        Task<IEnumerable<Job>> GetAllFilteredAsync(
           Expression<Func<Job, bool>>? filter = null,
           string? includeProperties = null,
           int? cityId = null,
           int? jobtypeId = null,
           string? keyword = null
        );

    }
}
