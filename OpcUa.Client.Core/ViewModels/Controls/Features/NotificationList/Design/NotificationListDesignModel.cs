using System.Collections.Generic;

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
            
            //Items = new List<NotificationMessageViewModel>
            //{
            //    new NotificationMessageViewModel(null)
            //    {
            //        Name = "ns=asdad;a=54a545",
            //        Message = "This chat app is awesome! I bet it will be fast too",
            //    },
            //    new NotificationMessageViewModel(null)
            //    {
            //        Name = "ns=asdad;a=54a545",
            //        Message = "This chat app is awesome! I bet it will be fast too",
            //    },
            //    new NotificationMessageViewModel(null)
            //    {
            //        Name = "ns=asdad;a=54a545",
            //        Message = "This chat app is awesome! I bet it will be fast too",
            //    },
            //    new NotificationMessageViewModel(null)
            //    {
            //        Name = "ns=asdad;a=54a545",
            //        Message = "This chat app is awesome! I bet it will be fast too",
            //    },
            //};
        }

        #endregion
    }
}
