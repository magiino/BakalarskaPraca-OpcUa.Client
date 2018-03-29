﻿using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class EndpointDataGrid : BaseViewModel
    {
        #region Public Properties
        public EndpointDescription EndpointDesciption { get; set; }

        public string EndpointUrl => EndpointDesciption.EndpointUrl;

        // TODO find or create enums for security
        public MessageSecurityMode SecurityMode => EndpointDesciption.SecurityMode;

        public string SecurityPolicy => SecurityPolicies.GetDisplayName(EndpointDesciption.SecurityPolicyUri);

        public int SecurityLevel => EndpointDesciption.SecurityLevel;

        public string TransportProfileUri => Profiles.NormalizeUri(EndpointDesciption.TransportProfileUri).Split('/').Last();

        // TODO Tokens
        public IEnumerable<UserTokenType> Tokens { get; set; }

        #endregion

        #region Constructor

        public EndpointDataGrid(EndpointDescription endpointDesciption)
        {
            EndpointDesciption = endpointDesciption;
            Tokens = EndpointDesciption.UserIdentityTokens.Select(x => x.TokenType).ToList();
        }

        #endregion
    }
}
