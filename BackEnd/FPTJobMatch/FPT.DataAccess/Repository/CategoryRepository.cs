using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;

namespace FPT.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByStatus(bool isApproved)
        {
            return await _db.Categories.Where(c => c.IsApproved == isApproved).ToListAsync();
        }

        public int CountCategories(IEnumerable<Category> categories, bool isThisMonth)
        {
            if (isThisMonth)
            {
                var today = DateTime.Today;
                return categories.Count(c => c.CreatedAt.Month == today.Month && c.CreatedAt.Year == today.Year);
            }
            else
            {
                return categories.Count();
            }
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }

        public async Task NullifyCreatedByUserIdAsync(string userId)
        {
            var categories = await _db.Categories.Where(c => c.CreatedByUserId == userId).ToListAsync();
            foreach (var category in categories)
            {
                category.CreatedByUserId = null;
            }
        }
    }
}
