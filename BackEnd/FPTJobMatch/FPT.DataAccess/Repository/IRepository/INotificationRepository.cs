using FPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        public void Update(Notification notification);
        Task RemoveBySenderIdAsync(string senderId);
        Task RemoveByReceiverIdAsync(string receiverId);
    }
}
