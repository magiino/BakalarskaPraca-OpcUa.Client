using System;
using System.Collections.ObjectModel;

namespace OpcUa.Client.WPF
{
    public class NotificationListDesignModel : NotificationListViewModel
    {
        #region Singleton
        public static NotificationListDesignModel Instance => new NotificationListDesignModel();
        #endregion

        #region Constructor
        public NotificationListDesignModel()
        {
            Items = new ObservableCollection<NotificationMessageViewModel>()
            {
                new NotificationMessageViewModel
                {
                    Name = "Pocet Bedniciek",
                    NodeId = "ns=3;s=Int16DataItem",
                    Message = "Hodnota sa zmenila o 66!",
                    Time = new DateTime()
                },
                new NotificationMessageViewModel
                {
                    Name = "Pocet Bedniciek",
                    NodeId = "ns=3;s=Int16DataItem",
                    Message = "Hodnota sa zmenila o 66!",
                    Time = new DateTime()
                },
                new NotificationMessageViewModel
                {
                    Name = "Pocet Bedniciek",
                    NodeId = "ns=3;s=Int16DataItem",
                    Message = "Hodnota sa zmenila o 66!",
                    Time = new DateTime()
                },
                new NotificationMessageViewModel
                {
                    Name = "Pocet Bedniciek",
                    NodeId = "ns=3;s=Int16DataItem",
                    Message = "Hodnota sa zmenila o 66!",
                    Time = new DateTime()
                },
            };
        }
        #endregion
    }
}