namespace OpcUa.Client.Core
{
    public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
    {
        public NotificationRepository(DataContext dataDontext): base(dataDontext) {}
    }
}