using System;
using System.Collections.ObjectModel;

namespace OpcUa.Client.WPF
{
    public class NotificationListDesignModel : NotificationListViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static NotificationListDesignModel Instance => new NotificationListDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
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
