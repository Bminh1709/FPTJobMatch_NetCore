using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository
{
    public class HelpArticleRepository : Repository<HelpArticle>, IHelpArticleRepository
    {
        private ApplicationDbContext _db;
        public HelpArticleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<HelpArticle>> GetAllArticlesFilteredAsync(string? sortType = null, string? keyword = null)
        {
            IQueryable<HelpArticle> query = _db.HelpArticles;

            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType == "NewestFirst")
                {
                    query = query.OrderByDescending(a => a.UpdatedAt);
                }
                else if (sortType == "OldestFirst")
                {
                    query = query.OrderBy(a => a.UpdatedAt);
                }
            }

            // Apply keyword search if keyword is provided
            if (!string.IsNullOrEmpty(keyword))
            {
                // Filter articles by title containing the keyword
                query = query.Where(a => a.Title.Contains(keyword));
            }

            // Execute the query and return the results asynchronously
            return await query.ToListAsync();
        }

        public void Update(HelpArticle helpArticle)
        {
            _db.HelpArticles.Update(helpArticle);
        }
    }
}
