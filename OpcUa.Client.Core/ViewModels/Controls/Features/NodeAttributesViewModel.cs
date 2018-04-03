using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class NodeAttributesViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;

        #endregion

        #region Public Properties
        //public ObservableCollection<AttributeListModel> SelectedNodeAttributes { get; set; } = new ObservableCollection<AttributeListModel>();

        public ExpandedNodeId NodeId { get; set; }
        public ReferenceDescription ReferenceDescription { get; set; }
        public Node Node { get; set; }
        public VariableNode VariableNode { get; set; }
        public NodeId DataTypeNodeId { get; set; }
        public DataValue DataValue { get; set; }
        public Type DataType { get; set; }
        public BuiltInType BuiltInType { get; set; }
        public bool IsVariableType { get; set; }
        public string ValueToWrite { get; set; }

        #endregion

        #region Commands

        public ICommand WriteValueCommand { get; set; }
        public ICommand ReadValueCommand { get; set; }

        #endregion

        #region Constructor


        // TODO vracat z write value ci sa podaril zapis
        public NodeAttributesViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

            WriteValueCommand = new RelayCommand(WriteValue);
            ReadValueCommand = new RelayCommand(ReadValue);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                msg =>
                {
                    ReferenceDescription = msg.ReferenceNode;
                    UpdateValues(ReferenceDescription);
                    //SelectedNodeAttributes = GetDataGridModel(ReferenceDescription);
                });
        }

        #endregion

        #region Command Methods

        private void WriteValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, new NamespaceTable());

            var value = _uaClientApi.WriteValue(nodeId, BuiltInType,  ValueToWrite);

            if (value == null) return;

            DataValue = value;
        }

        private void ReadValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, new NamespaceTable());
            DataValue = _uaClientApi.ReadValue(nodeId);
        }

        #endregion

        #region Private Helpers

        private void UpdateValues(ReferenceDescription referenceDescription)
        {
            IsVariableType = false;

            NodeId = referenceDescription.NodeId;
            Node = _uaClientApi.ReadNode(NodeId);

            if (Node.NodeClass != NodeClass.Variable) return;

            IsVariableType = true;
            VariableNode = (VariableNode)Node.DataLock;
            DataTypeNodeId = VariableNode.DataType;
            DataValue = _uaClientApi.ReadValue(VariableNode.NodeId);
            DataType = TypeInfo.GetSystemType(VariableNode.DataType, new EncodeableFactory());
            BuiltInType = TypeInfo.GetBuiltInType(VariableNode.DataType);
        }

        //private ObservableCollection<AttributeListModel> GetDataGridModel(ReferenceDescription referenceDescription)
        //{
        //    var data = new ObservableCollection<AttributeListModel>();

        //    var tmpNodeId = referenceDescription.NodeId;

        //    data.Add(new AttributeListModel("Node Id", tmpNodeId));
        //    data.Add(new AttributeListModel("Namespace Index", tmpNodeId.NamespaceIndex));
        //    data.Add(new AttributeListModel("Type", tmpNodeId.IdType));
        //    data.Add(new AttributeListModel("Identifier", tmpNodeId.Identifier));
        //    data.Add(new AttributeListModel("Node Class", referenceDescription.NodeClass));
        //    data.Add(new AttributeListModel("Browse Name", referenceDescription.BrowseName));
        //    data.Add(new AttributeListModel("Display Name", referenceDescription.DisplayName));

        //    var node = _uaClientApi.ReadNode(tmpNodeId);

        //    data.Add(new AttributeListModel("Description", node.Description));
        //    data.Add(new AttributeListModel("Write Mask", node.WriteMask));
        //    data.Add(new AttributeListModel("User Write Mask", node.UserWriteMask));

        //    if (node.NodeClass != NodeClass.Variable) return data;

        //    var variableNode = (VariableNode)node.DataLock;
        //    data.Add(new AttributeListModel("Value Rank", variableNode.ValueRank));
        //    data.Add(new AttributeListModel("Data Type", variableNode.DataType));
        //    data.Add(new AttributeListModel("Namespace Index", variableNode.DataType.NamespaceIndex));
        //    data.Add(new AttributeListModel("Identifier", variableNode.DataType.Identifier));
        //    data.Add(new AttributeListModel("Id Type", variableNode.DataType.IdType));
        //    data.Add(new AttributeListModel("Array Dimensions", variableNode.ArrayDimensions));
        //    data.Add(new AttributeListModel("Access Level", variableNode.AccessLevel));
        //    data.Add(new AttributeListModel("User Access Level", variableNode.UserAccessLevel));
        //    data.Add(new AttributeListModel("Historozing", variableNode.Historizing));
        //    data.Add(new AttributeListModel("Minimum Sampling", variableNode.MinimumSamplingInterval));
        //    data.Add(new AttributeListModel("Value", _uaClientApi.ReadValue(variableNode.NodeId)));
        //    data.Add(new AttributeListModel("Data Type", TypeInfo.GetSystemType(variableNode.DataType, new EncodeableFactory())));
        //    data.Add(new AttributeListModel("Built In Type", TypeInfo.GetBuiltInType(variableNode.DataType)));

        //    return data;
        //}

        #endregion
    }
}
