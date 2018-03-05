using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class EndpointDataGridModel
    {
        #region Private Fields

        public EndpointDescription EndpointDesciption { get; set; }

        #endregion

        #region Public Properties

        public string ApplicationName => EndpointDesciption.Server.ApplicationName.ToString();

        // TODO find or create enums for security
        public MessageSecurityMode SecurityMode => EndpointDesciption.SecurityMode;

        public string SecurityPolicy => SecurityPolicies.GetDisplayName(EndpointDesciption.SecurityPolicyUri);

        public string EndpointUrl => EndpointDesciption.EndpointUrl;

        #endregion

        #region Constructor

        public EndpointDataGridModel(EndpointDescription endpointDesciption)
        {
            EndpointDesciption = endpointDesciption;
        }

        #endregion
    }
}
