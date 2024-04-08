using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface IStatusRepository : IRepository<Status>
    {
        public void Update(Status accountStatus);
    }
}
