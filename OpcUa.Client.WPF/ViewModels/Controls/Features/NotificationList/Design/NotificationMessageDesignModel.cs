using System;

namespace OpcUa.Client.WPF
{
    public class NotificationMessageDesignModel : NotificationMessageViewModel
    {
        #region Singleton
        public static NotificationMessageDesignModel Instance => new NotificationMessageDesignModel();
        #endregion

        #region Constructor
        public NotificationMessageDesignModel()
        {
            Name = "Pocet Bedniciek";
            NodeId = "ns=3;s=Int16DataItem";
            Message = "Hodnota sa zmenila o 66!";
            Time = new DateTime();
        }
        #endregion
    }
}