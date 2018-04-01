using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public static class Mapper
    {
        public static ICollection<ProjectModel> ProjectToListModel(ICollection<ProjectEntity> projectEntities)
        {
            return projectEntities.Select(x => new ProjectModel()
            {
                ProjectId = x.Id,
                Name = x.Name,
                EndpointId = x.EndpointId,
                EndpointUrl = x.Endpoint.Url,
                SessionName = x.SessionName,
                UserId = x.UserId
            }).ToList();
        }

        public static EndpointDescription CreateEndpointDescription(EndpointEntity endpoint)
        {
            return new EndpointDescription()
            {
                EndpointUrl = endpoint.Url,
                SecurityPolicyUri = endpoint.SecurityPolicyUri,
                SecurityMode = endpoint.MessageSecurityMode,
                TransportProfileUri = endpoint.TransportProfileUri
            };
        }
    }
}
