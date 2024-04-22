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
    public class HelpTypeRepository : Repository<HelpType>, IHelpTypeRepository
    {
        private ApplicationDbContext _db;
        public HelpTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> IsExists(string name)
        {
            return await _db.HelpTypes.AnyAsync(e => e.Name.ToLower() == name.ToLower());
        }

        public void Update(HelpType helpType)
        {
            _db.HelpTypes.Update(helpType);
        }
    }
}
