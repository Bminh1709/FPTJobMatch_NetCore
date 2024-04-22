using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;

namespace FPT.DataAccess.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Notification notification)
        {
            _db.Notifications.Update(notification);
        }

        public async Task RemoveBySenderIdAsync(string senderId)
        {
            var notifications = await _db.Notifications.Where(n => n.SenderId == senderId).ToListAsync();
            _db.Notifications.RemoveRange(notifications);
        }

        public async Task RemoveByReceiverIdAsync(string receiverId)
        {
            var notifications = await _db.Notifications.Where(n => n.ReceiverId == receiverId).ToListAsync();
            _db.Notifications.RemoveRange(notifications);
        }
    }
}
