using System;
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
        public ICommand AddNotificationCommand { get; }
        public ICommand RemoveNotificationCommand { get; }
        public ICommand DeleteAllNotificationsCommand { get; }
        #endregion

        #region Constructor
        public NotificationViewModel(IUnitOfWork unitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            _uaClientApi = uaClientApi;
            _unitOfWork = unitOfWork;
            _messenger = messenger;

            _subscription = _uaClientApi.Subscribe(300, "Notifications");

            if (_subscription == null)
                IoC.AppManager.ShowWarningMessage("Subscription creation failed, please restart application!");

            LoadAndRegisterNotifications();

            NotificationListVm = new NotificationListViewModel();

            AddNotificationCommand = new MixRelayCommand(AddNotification, AddNotificationCanUse);
            RemoveNotificationCommand = new MixRelayCommand(RemoveNotification, RemoveNotificationCanUse);
            DeleteAllNotificationsCommand = new MixRelayCommand(DeleteAllNotifications, DeleteAllCanUse);

            _messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);

            _messenger.Register<SendNewNotification>(msg => AddNotificationToSubscription(msg.Notification));
        }

        #endregion

        #region Command Methods
        private void AddNotification(object parameter)
        {
            var isDigital = _uaClientApi.GetBuiltInTypeOfVariableNodeId(_selectedNode.NodeId.ToString()) == BuiltInType.Boolean;

            IoC.Ui.ShowAddNotification(new AddNotificationDialogViewModel(_messenger)
            {
                NodeId = _selectedNode.NodeId.ToString(),
                IsDigital = isDigital
            });
        }

        private void RemoveNotification(object parameter)
        {
            try
            {
                _uaClientApi.RemoveMonitoredItem(_subscription, SelectedNotification.NodeId);
                Notifications.Remove(SelectedNotification);
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
        }

        private void DeleteAllNotifications(object parameter)
        {
            NotificationListVm.Items.Clear();
        }

        #endregion

        #region Can use methods
        public bool AddNotificationCanUse(object parameter)
        {
            if (_selectedNode == null)
                return false;
            else if (_selectedNode.NodeClass != NodeClass.Variable)
                return false;
            else return true;
        }

        public bool RemoveNotificationCanUse(object parameter)
        {
            return SelectedNotification != null;
        }

        public bool DeleteAllCanUse(object parameter)
        {
            return NotificationListVm.Items.Count != 0;
        }

        #endregion

        #region Helpers
        private void AddNotificationToSubscription(ExtendedNotificationModel notification)
        {
            notification.DataType = _uaClientApi.GetBuiltInTypeOfVariableNodeId(notification.NodeId);

            MonitoredItem item;

            var notificationEntity = new NotificationEntity()
            {
                Id = notification.Id ?? -1,
                Name = notification.Name,
                NodeId = notification.NodeId,
                ProjectId = IoC.AppManager.ProjectId,
            };

            if (notification.IsDigital)
            {
                item = _uaClientApi.CreateMonitoredItem(notification.Name, notification.NodeId, 100);

                notificationEntity.IsDigital = true;
                notificationEntity.IsZeroDescription = notification.IsZeroDescription;
                notificationEntity.IsOneDescription = notification.IsOneDescription;
            }
            else
            {
                item = _uaClientApi.CreateMonitoredItem(notification.Name,
                    notification.NodeId,
                    100,
                    new DataChangeFilter()
                    {
                        DeadbandType = (uint)notification.DeadbandType,
                        DeadbandValue = notification.FilterValue,
                        Trigger = DataChangeTrigger.StatusValue
                    });

                notificationEntity.FilterValue = notification.FilterValue;
                notificationEntity.DeadbandType = notification.DeadbandType;
            }

            if (_uaClientApi.AddMonitoredItem(item, _subscription) == false)
                IoC.AppManager.ShowWarningMessage("Notification could not be created!");
            else
            {
                item.Notification += Notification_MonitoredItem;
                if (notification.Id == null)
                    notification.Id = _unitOfWork.Notifications.Add(notificationEntity).Id;
                Notifications.Add(notification);
            }
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
                message = $"Hodnota premennej sa zmenila o {variable.FilterValue} [{variable.DeadbandType.ToString()}] na {notification.Value.Value}. ";

            _messenger.Send(new SendNotificationAdd(variable.Name, variable.NodeId,message, value.SourceTimestamp));
        }
        #endregion
    }
}