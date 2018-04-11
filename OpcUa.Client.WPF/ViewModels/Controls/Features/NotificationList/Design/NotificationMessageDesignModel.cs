namespace OpcUa.Client.WPF
{
    public class NotificationMessageDesignModel : NotificationMessageViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static NotificationMessageDesignModel Instance => new NotificationMessageDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationMessageDesignModel()
        {
            Name = "ns=asdad;a=54a545";
            Message = "This chat app is awesome! I bet it will be fast too";
        }

        #endregion
    }
}
