using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        // only modifying data in memory, without interacting directly with the database
        void Update(ApplicationUser applicationUser);
    }
}
