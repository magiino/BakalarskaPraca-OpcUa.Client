namespace OpcUa.Client.Core
{
    public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
    {
        private DataContext DataContect => Context as DataContext;
        public NotificationRepository(DataContext dataDontext): base(dataDontext) {}
    }
}