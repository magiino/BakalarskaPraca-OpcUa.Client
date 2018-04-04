using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public static class Mapper
    {
        public static ICollection<ProjectModel> ProjectToListModel(IEnumerable<ProjectEntity> projectEntities)
        {
            return projectEntities.Select(x => new ProjectModel()
            {
                Id = x.Id,
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

        public static EndpointEntity CreateEndpointEntity(EndpointDescription endpoint)
        {
            return new EndpointEntity()
            {
                Url = endpoint.EndpointUrl,
                MessageSecurityMode = endpoint.SecurityMode,
                SecurityPolicyUri = endpoint.SecurityPolicyUri,
                TransportProfileUri = endpoint.TransportProfileUri
            };
        }

        public static ICollection<ExtendedNotificationModel> NotificationsToExtended(IEnumerable<NotificationEntity> notifications)
        {
            return notifications.Select(notification => new ExtendedNotificationModel()
                {
                    Name = notification.Name,
                    NodeId = notification.NodeId,
                    IsDigital = notification.IsDigital,
                    IsOneDescription = notification.IsOneDescription,
                    IsZeroDescription = notification.IsZeroDescription,
                    FilterValue = notification.FilterValue,
                    DeadbandType = notification.DeadbandType
                }).ToList();
        }
    }
}
