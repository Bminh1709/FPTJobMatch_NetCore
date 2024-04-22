using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IHelpTypeRepository : IRepository<HelpType>
    {
        public void Update(HelpType helpType);
        Task<bool> IsExists(string name);
    }
}
