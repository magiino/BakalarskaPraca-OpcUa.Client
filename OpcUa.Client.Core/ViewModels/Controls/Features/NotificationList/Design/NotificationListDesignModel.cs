using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpcUa.Client.Core
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
                    Name = "ns=asdad;a=54a545",
                    Message = "This chat app is awesome! I bet it will be fast too",
                },
                new NotificationMessageViewModel
                {
                    Name = "ns=asdad;a=54a545",
                    Message = "This chat app is awesome! I bet it will be fast too",
                },
                new NotificationMessageViewModel
                {
                    Name = "ns=asdad;a=54a545",
                    Message = "This chat app is awesome! I bet it will be fast too",
                },
                new NotificationMessageViewModel
                {
                    Name = "ns=asdad;a=54a545",
                    Message = "This chat app is awesome! I bet it will be fast too",
                },
            };
        }

        #endregion
    }
}
