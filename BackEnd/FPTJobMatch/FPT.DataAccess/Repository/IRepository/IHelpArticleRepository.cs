using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IHelpArticleRepository : IRepository<HelpArticle>
    {
        public void Update(HelpArticle helpArticle);
        Task<IEnumerable<HelpArticle>> GetAllArticlesFilteredAsync(string? sortType = null, string? keyword = null);
    }
}
