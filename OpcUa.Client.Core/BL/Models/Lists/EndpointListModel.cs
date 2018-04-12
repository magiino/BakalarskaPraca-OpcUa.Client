using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class EndpointListModel : BaseViewModel
    {
        #region Public Properties
        public EndpointDescription EndpointDesciption { get; set; }

        public string EndpointUrl => EndpointDesciption.EndpointUrl;

        public MessageSecurityMode SecurityMode => EndpointDesciption.SecurityMode;

        public string SecurityPolicy => SecurityPolicies.GetDisplayName(EndpointDesciption.SecurityPolicyUri);

        public int SecurityLevel => EndpointDesciption.SecurityLevel;

        public string TransportProfileUri => Profiles.NormalizeUri(EndpointDesciption.TransportProfileUri).Split('/').Last();

        #endregion

        #region Constructor

        public EndpointListModel(EndpointDescription endpointDesciption)
        {
            EndpointDesciption = endpointDesciption;
        }

        #endregion
    }
}
