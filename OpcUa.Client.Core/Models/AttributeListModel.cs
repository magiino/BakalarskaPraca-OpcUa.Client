namespace OpcUa.Client.Core
{
    /// <summary>
    /// Data model for attribute data grid
    /// </summary>
    public class AttributeListModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Attribute of ReferenceDescription object
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Value of ReferenceDescription object Attribute
        /// </summary>
        public object Value { get; set; }

        #endregion

        #region Constructor

        public AttributeListModel(string attribute, object value)
        {
            Attribute = attribute;
            Value = value;
        } 

        #endregion

    }
}
