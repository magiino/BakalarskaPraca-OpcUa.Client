using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public static class Mapper
    {
        // Projects
        public static ICollection<ProjectModel> ProjectEntityToProjectListModel(IEnumerable<ProjectEntity> projectEntities)
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

        // Variables
        public static ICollection<VariableModel> VariableEntitiesToVariableListModels(IEnumerable<VariableEntity> variableEntities)
        {
            return variableEntities.Select(x => new VariableModel()
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                Name = x.Name,
                Archive = x.Archive,
                DataType = x.DataType
            }).ToList();
        }

        public static VariableModel VariableEntityToVariableModel(VariableEntity variableEntity)
        {
            return new VariableModel()
            {
                Name = variableEntity.Name,
                DataType = variableEntity.DataType,
                ProjectId = variableEntity.ProjectId,
                Archive = variableEntity.Archive
            };
        }

        public static VariableEntity VariableModelToVariableEntity(VariableModel variableModel)
        {
            return new VariableEntity()
            {
                Id = variableModel.Id,
                Name = variableModel.Name,
                DataType = variableModel.DataType,
                ProjectId = variableModel.ProjectId,
                Archive = variableModel.Archive
            };
        }

        // Endpoints
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

        // Notifications
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
