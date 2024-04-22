using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public void Update(Category category);
        Task<IEnumerable<Category>> GetCategoriesByStatus(bool isApproved);
        Task NullifyCreatedByUserIdAsync(string userId);
        int CountCategories(IEnumerable<Category> categories, bool isThisMonth);
    }
}
