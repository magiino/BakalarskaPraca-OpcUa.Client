using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class NodeAttributesViewModel : BaseViewModel
    {
        private readonly UaClientApi _uaClientApi;

        private ReferenceDescription _refDiscOfSelectedNode;

        #region Public Properties
        public ObservableCollection<AttributeDataGridViewModel> SelectedNode { get; set; } = new ObservableCollection<AttributeDataGridViewModel>();

        #endregion

        #region Commands

        public ICommand WriteSingleValueCommand { get; set; }
        public ICommand ReadSingleValueCommand { get; set; }
        public string ValueToSingleWrite { get; set; }

        #endregion

        #region Constructor

        public NodeAttributesViewModel()
        {
            _uaClientApi = IoC.UaClientApi;
            // TODO prerobit na parametrizovany command?
            WriteSingleValueCommand = new RelayCommand(WriteSingleValue);
            ReadSingleValueCommand = new RelayCommand(ReadSingleValue);

            MessengerInstance.Register<SendSelectedRefNode>(this, node => { _refDiscOfSelectedNode = node.RefNode;});

        }

        private void WriteSingleValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(_refDiscOfSelectedNode.NodeId, null);
            var variable = new Variable()
            {
                MonitoredItem = new MonitoredItem()
                {
                    StartNodeId = nodeId
                }
            };
            var data = _uaClientApi.ReadValue(nodeId);
            variable.Value = data.Value;

            bool writeStatus = _uaClientApi.WriteValue(variable, ValueToSingleWrite);

            if (data.StatusCode.Code != StatusCodes.Good || !writeStatus) return;
            foreach (var atribute in SelectedNode)
            {
                if (atribute.Attribute == "Value")
                    atribute.Value = ValueToSingleWrite;
            }
        }

        private void ReadSingleValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(_refDiscOfSelectedNode.NodeId, null);
            var data = _uaClientApi.ReadValue(nodeId);
            if (data.StatusCode.Code != StatusCodes.Good) return;
            foreach (var atribute in SelectedNode)
            {
                if (atribute.Attribute == "Value")
                    atribute.Value = data.Value;
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Takes attributes and values of <see cref="ReferenceDescription"/> object
        /// </summary>
        /// <param name="referenceDescription"></param>
        /// <returns></returns>
        private ObservableCollection<AttributeDataGridViewModel> GetDataGridModel(ReferenceDescription referenceDescription)
        {
            var data = new ObservableCollection<AttributeDataGridViewModel>();

            foreach (var propertyInfo in referenceDescription.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(referenceDescription);

                if (value is NodeId)
                    value.GetType().GetProperties().ToList().ForEach(property => data.Add(new AttributeDataGridViewModel(property.Name, property.GetValue(value).ToString())));

                data.Add(new AttributeDataGridViewModel(propertyInfo.Name, value.ToString()));
            }

            var node = _uaClientApi.ReadNode(referenceDescription.NodeId.ToString());
            node.GetType().GetProperties().ToList().ForEach(property => data.Add(new AttributeDataGridViewModel(property.Name, property.GetValue(node)?.ToString())));

            if (node.NodeClass != NodeClass.Variable) return data;

            var variableNode = (VariableNode)node.DataLock;
            variableNode.GetType().GetProperties().ToList().ForEach(property =>
            data.Add(new AttributeDataGridViewModel(property.Name, property.GetValue(variableNode)?.ToString())));

            return data;
        }

        /// <summary>
        /// Takes attributes and values of <see cref="ReferenceDescription"/> object
        /// </summary>
        /// <param name="referenceDescription"></param>
        /// <returns></returns>
        private ObservableCollection<AttributeDataGridViewModel> GetDataGridModel2(ReferenceDescription referenceDescription)
        {
            var data = new ObservableCollection<AttributeDataGridViewModel>();

            var tmpNodeId = referenceDescription.NodeId;

            data.Add(new AttributeDataGridViewModel("Node Id", tmpNodeId));
            data.Add(new AttributeDataGridViewModel("Namespace Index", tmpNodeId.NamespaceIndex));
            data.Add(new AttributeDataGridViewModel("Type", tmpNodeId.IdType));
            data.Add(new AttributeDataGridViewModel("Node Id", tmpNodeId.Identifier));
            data.Add(new AttributeDataGridViewModel("Node Class", referenceDescription.NodeClass));
            data.Add(new AttributeDataGridViewModel("Browse Name", referenceDescription.BrowseName));
            data.Add(new AttributeDataGridViewModel("Display Name", referenceDescription.DisplayName));

            var node = _uaClientApi.ReadNode(tmpNodeId.ToString());

            data.Add(new AttributeDataGridViewModel("Description", node.Description));
            data.Add(new AttributeDataGridViewModel("Write Mask", node.WriteMask));
            data.Add(new AttributeDataGridViewModel("User Write Mask", node.UserWriteMask));

            if (node.NodeClass != NodeClass.Variable) return data;

            var variableNode = (VariableNode)node.DataLock;
            data.Add(new AttributeDataGridViewModel("Value Rank", variableNode.ValueRank));
            data.Add(new AttributeDataGridViewModel("Data Type Node Id", variableNode.DataType));
            data.Add(new AttributeDataGridViewModel("Namespace Index", variableNode.DataType.NamespaceIndex));
            data.Add(new AttributeDataGridViewModel("Identifier", variableNode.DataType.Identifier));
            data.Add(new AttributeDataGridViewModel("Id Type", variableNode.DataType.IdType));
            data.Add(new AttributeDataGridViewModel("Array Dimensions", variableNode.ArrayDimensions));
            data.Add(new AttributeDataGridViewModel("Access Level", variableNode.AccessLevel));
            data.Add(new AttributeDataGridViewModel("User Access Level", variableNode.UserAccessLevel));
            data.Add(new AttributeDataGridViewModel("Historozing", variableNode.Historizing));
            data.Add(new AttributeDataGridViewModel("minimum Sampling", variableNode.MinimumSamplingInterval));
            data.Add(new AttributeDataGridViewModel("Value", _uaClientApi.ReadValue(variableNode.NodeId)));
            data.Add(new AttributeDataGridViewModel("Data Type", TypeInfo.GetSystemType(variableNode.DataType, new EncodeableFactory())));
            data.Add(new AttributeDataGridViewModel("Built In Type", TypeInfo.GetBuiltInType(variableNode.DataType)));

            return data;
        }
        #endregion
    }
}
