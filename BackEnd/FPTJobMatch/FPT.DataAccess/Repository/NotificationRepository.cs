using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;

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
    }
}
