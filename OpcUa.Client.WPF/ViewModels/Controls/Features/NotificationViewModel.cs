using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class NotificationViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;
        private readonly Messenger _messenger;
        private readonly IUnitOfWork _unitOfWork;
        private ReferenceDescription _selectedNode;
        private readonly Subscription _subscription; 

        #endregion

        #region Public Properties

        public NotificationListViewModel NotificationListVm { get; set; }
        public ObservableCollection<ExtendedNotificationModel> Notifications { get; set; } = new ObservableCollection<ExtendedNotificationModel>();
        public ExtendedNotificationModel SelectedNotification { get; set; }

        #endregion

        #region Commands

        public ICommand AddNotificationCommand { get; set; }
        public ICommand RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        public NotificationViewModel(IUnitOfWork unitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            _uaClientApi = uaClientApi;
            _unitOfWork = unitOfWork;
            _messenger = messenger;

            _subscription = _uaClientApi.Subscribe(300, "Notifications");
            LoadAndRegisterNotifications();

            NotificationListVm = new NotificationListViewModel();

            //AddNotificationCommand = new RelayCommand(AddNotification);
            AddNotificationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(AddNotification, AddNotificationCanUse);
            //RemoveNotificationCommand = new RelayCommand(RemoveNotification);
            RemoveNotificationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(RemoveNotification, RemoveNotificationCanUse);

            _messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);

            _messenger.Register<SendNewNotification>(msg => AddNotificationToSubscription(msg.Notification));
        }

        #endregion

        #region Command Methods

        private void AddNotification()
        {
            var isDigital = _uaClientApi.GetBuiltInTypeOfVariableNodeId(_selectedNode.NodeId.ToString()) == BuiltInType.Boolean;

            IoC.Ui.ShowAddNotification(new AddNotificationDialogViewModel(_messenger)
            {
                NodeId = _selectedNode.NodeId.ToString(),
                IsDigital = isDigital
            });
        }

        private void RemoveNotification()
        {
            _uaClientApi.RemoveMonitoredItem(_subscription, SelectedNotification.NodeId);
            Notifications.Remove(SelectedNotification);
        }

        #endregion

        #region Can use methods

        public bool AddNotificationCanUse()
        {
            if (_selectedNode == null)
                return false;
            else if (_selectedNode.NodeClass != NodeClass.Variable)
                return false;
            else return true;
        }

        public bool RemoveNotificationCanUse()
        {
            return SelectedNotification != null;
        }

        #endregion

        #region Helpers

        private void AddNotificationToSubscription(ExtendedNotificationModel notification)
        {
            notification.DataType = _uaClientApi.GetBuiltInTypeOfVariableNodeId(notification.NodeId);

            MonitoredItem item;

           var notificationEntity = new NotificationEntity()
            {
                Name = notification.Name,
                NodeId = notification.NodeId,
                ProjectId = IoC.AppManager.ProjectId,
            };

            if (notification.IsDigital)
            {
                item = _uaClientApi.CreateMonitoredItem(notification.Name, notification.NodeId, 300);

                notificationEntity.IsDigital = true;
                notificationEntity.IsZeroDescription = notification.IsZeroDescription;
                notificationEntity.IsOneDescription = notification.IsOneDescription;
            }
            else
            {
                item = _uaClientApi.CreateMonitoredItem(notification.Name,
                    notification.NodeId,
                    300,
                    new DataChangeFilter()
                    {
                        DeadbandType = (uint)notification.DeadbandType,
                        DeadbandValue = notification.FilterValue,
                        Trigger = DataChangeTrigger.StatusValue
                    });

                notificationEntity.FilterValue = notification.FilterValue;
                notificationEntity.DeadbandType = notification.DeadbandType;
            }

            _uaClientApi.AddMonitoredItem(item, _subscription);
            item.Notification += Notification_MonitoredItem;

            _unitOfWork.Notifications.Add(notificationEntity);

            Notifications.Add(notification);
        }

        private void LoadAndRegisterNotifications()
        {
            var extendedNotifications =
                Mapper.NotificationsToExtended(_unitOfWork.Notifications.Find(x => x.ProjectId == IoC.AppManager.ProjectId));

            foreach (var extendedNonotification in extendedNotifications)
                AddNotificationToSubscription(extendedNonotification);
        }

        #endregion

        #region CallBack Methods

        /// <summary>
        /// Callback method for updating values of subscibed nodes
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (!(e.NotificationValue is MonitoredItemNotification notification))
                return;

            var value = notification.Value;

            var variable = Notifications.FirstOrDefault(x => x.Name == monitoredItem.DisplayName);

            if (variable == null) return;

            var message = "";
            if (variable.IsDigital && (bool)value.Value)
                message = variable.IsOneDescription;
            else if (variable.IsDigital && !(bool)value.Value)
                message = variable.IsZeroDescription;
            else if (!variable.IsDigital)
                message = $"Hodnota premennej {variable.Name} sa zmenila o {notification.Value.Value} {variable.DeadbandType.ToString()}";

            _messenger.Send(new SendNotificationAdd(variable.Name, message, value.SourceTimestamp));
        }

        #endregion
    }
}