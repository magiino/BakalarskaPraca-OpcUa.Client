namespace OpcUA.Client.Core
{
    /// <summary>
    /// Data model for attribute data grid
    /// </summary>
    public class AttributeDataGridModel
    {
        #region Public Properties

        /// <summary>
        /// Attribute of ReferenceDescription object
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Value of ReferenceDescription object Attribute
        /// </summary>
        public string Value { get; set; }

        #endregion

        #region Constructor

        public AttributeDataGridModel(string attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        } 

        #endregion

    }
}
