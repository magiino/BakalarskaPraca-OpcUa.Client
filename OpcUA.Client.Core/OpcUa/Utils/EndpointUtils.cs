using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public static class EndpointUtils
    {
        private static readonly string[] Protocol = { "opc.tcp://", "http://", "https://"};

        public static string EProtocolToString(Protocol protocol)
        {
            return Protocol[(int)protocol];
        }


        private static readonly string[] SecurityPoliciesStringValues = { "None", "Basic128Rsa15", "Basic256", "Https", "Basic256Sha256" };

        public static string ESecutityPolicyToString(SecurityPolicy policy)
        {
            return SecurityPoliciesStringValues[(int)policy];
        }



        public static List<EndpointDescription> SelectByProtocol(List<EndpointDescription> endpoints, Protocol protocol)
        {
            return endpoints?.Where(endpoint => endpoint.EndpointUrl.ToLower().StartsWith( EProtocolToString(protocol)) ).ToList();
        }


        public static List<EndpointDescription> SelectByMessageSecurityMode(List<EndpointDescription> endpoints, MessageSecurityMode mode)
        {
            return endpoints?.Where(endpoint => endpoint.SecurityMode == mode).ToList();
        }

        public static List<EndpointDescription> SelectByMessageSecurityModes(List<EndpointDescription> endpoints, List<MessageSecurityMode> selectedModes)
        {
            return endpoints?.Where(endpoint => selectedModes.Contains(endpoint.SecurityMode) ).ToList();
        }


        public static List<EndpointDescription> SelectBySecurityPolicy(List<EndpointDescription> endpoints, string policy)
        {
            return endpoints.Where(endpoint => SecurityPolicies.GetDisplayName(endpoint.SecurityPolicyUri) == policy).ToList();
        }
        public static List<EndpointDescription> SelectBySecurityPolicies(List<EndpointDescription> endpoints, List<string> policies)
        {
            return endpoints.Where(endpoint => policies.Contains(SecurityPolicies.GetDisplayName(endpoint.SecurityPolicyUri)) ).ToList();
        }


        public static List<EndpointDescription> SelectByApplicationName(List<EndpointDescription> endpoints, string applicationName)
        {
            return endpoints.Where(endpoint => endpoint.Server.ApplicationName.ToString() == applicationName).ToList();
        }

        public static List<EndpointDescription> SelectByApplicationNames(List<EndpointDescription> endpoints, string[] applicationNames)
        {
            return endpoints.Where(endpoint => applicationNames.Contains(endpoint.Server.ApplicationName.ToString()) ).ToList();
        }


        public static List<EndpointDescription> SelectByEncoding(List<EndpointDescription> endpoints, string[] applicationNames)
        {
            return endpoints.Where(endpoint => applicationNames.Contains(endpoint.Server.ApplicationName.ToString())).ToList();
        }

    }
}
