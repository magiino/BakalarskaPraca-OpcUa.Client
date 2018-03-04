using Opc.Ua;

namespace OpcUA.Client.Core.DataModels
{
    public class EndpointDataGridModel
    {
        #region Private Fields

        private readonly EndpointDescription _endpointDesciption;

        #endregion

        #region Public Properties

        public string ApplicationName => _endpointDesciption.Server.ApplicationName.ToString();

        // TODO find or create enums for security
        public MessageSecurityMode SecurityMode => _endpointDesciption.SecurityMode;

        public string SecurityPolicy => SecurityPolicies.GetDisplayName(_endpointDesciption.SecurityPolicyUri);

        public string EndpointUrl => _endpointDesciption.EndpointUrl;

        #endregion

        #region Constructor

        public EndpointDataGridModel(EndpointDescription endpointDesciption)
        {
            _endpointDesciption = endpointDesciption;
        }

        #endregion
    }
}
