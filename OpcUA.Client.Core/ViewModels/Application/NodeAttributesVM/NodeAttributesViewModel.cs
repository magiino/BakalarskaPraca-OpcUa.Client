using System;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class NodeAttributesViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly UaClientApi _uaClientApi;
        #endregion

        #region Public Properties

        public ExpandedNodeId NodeId { get; set; }
        public ReferenceDescription ReferenceDescription { get; set; }
        public Node Node { get; set; }
        public VariableNode VariableNode { get; set; }
        public NodeId DataTypeNodeId { get; set; }
        public DataValue DataValue { get; set; }
        public Type DataType { get; set; }
        public BuiltInType BuiltInType { get; set; }
        public bool IsVariableType { get; set; }
        public string ValueToSingleWrite { get; set; }

        public bool IsExpanded { get; set; }

        #endregion

        #region Commands

        public ICommand WriteSingleValueCommand { get; set; }
        public ICommand ReadSingleValueCommand { get; set; }

        #endregion

        #region Constructor

        // TODO prerobit _selectedNode z refDisc na NodeId
        // TODO Prerobit WriteValue v opcuaApi
        // TODO vracat z write value ci sa podaril zapis
        // TODO Stale tam zobrazovat atributy len menit hodnoty !!!
        // TODO Urobit ViewModel zvlast pre vsetko
        public NodeAttributesViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

            WriteSingleValueCommand = new RelayCommand(WriteSingleValue);
            ReadSingleValueCommand = new RelayCommand(ReadSingleValue);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                node =>
                {
                    ReferenceDescription = node.RefNode;
                    UpdateValues(ReferenceDescription);
                });
        }

        #endregion

        #region Command Methods

        private void WriteSingleValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, null);
            var variable = new Variable()
            {
                MonitoredItem = new MonitoredItem()
                {
                    StartNodeId = nodeId
                }
            };

            // TOTO robim aby som si zistil data type
            // TODO vymysliet to inak
            var data = _uaClientApi.ReadValue(nodeId);
            variable.Value = data.Value;

            bool writeStatus = _uaClientApi.WriteValue(variable, ValueToSingleWrite);

            if (data.StatusCode.Code != StatusCodes.Good || !writeStatus) return;
            DataValue.Value = ValueToSingleWrite;
        }

        private void ReadSingleValue()
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, null);
            DataValue = _uaClientApi.ReadValue(nodeId);
        }

        #endregion

        #region Private Helpers

        private void UpdateValues(ReferenceDescription referenceDescription)
        {
            IsVariableType = false;

            NodeId = referenceDescription.NodeId;
            Node = _uaClientApi.ReadNode(NodeId.ToString());

            if (Node.NodeClass != NodeClass.Variable) return;

            IsVariableType = true;
            VariableNode = (VariableNode)Node.DataLock;
            DataTypeNodeId = VariableNode.DataType;
            DataValue = _uaClientApi.ReadValue(VariableNode.NodeId);
            DataType = TypeInfo.GetSystemType(VariableNode.DataType, new EncodeableFactory());
            BuiltInType = TypeInfo.GetBuiltInType(VariableNode.DataType);
        }

        ///// <summary>
        ///// Takes attributes and values of <see cref="ReferenceDescription"/> object
        ///// </summary>
        ///// <param name="referenceDescription"></param>
        ///// <returns></returns>
        //private ObservableCollection<AttributeDataGridViewModel> GetDataGridModel(ReferenceDescription referenceDescription)
        //{
        //    var data = new ObservableCollection<AttributeDataGridViewModel>();

        //    var tmpNodeId = referenceDescription.NodeId;

        //    data.Add(new AttributeDataGridViewModel("Node Id", tmpNodeId));
        //    data.Add(new AttributeDataGridViewModel("Namespace Index", tmpNodeId.NamespaceIndex));
        //    data.Add(new AttributeDataGridViewModel("Type", tmpNodeId.IdType));
        //    data.Add(new AttributeDataGridViewModel("Identifier", tmpNodeId.Identifier));
        //    data.Add(new AttributeDataGridViewModel("Node Class", referenceDescription.NodeClass));
        //    data.Add(new AttributeDataGridViewModel("Browse Name", referenceDescription.BrowseName));
        //    data.Add(new AttributeDataGridViewModel("Display Name", referenceDescription.DisplayName));

        //    var node = _uaClientApi.ReadNode(tmpNodeId.ToString());

        //    data.Add(new AttributeDataGridViewModel("Description", node.Description));
        //    data.Add(new AttributeDataGridViewModel("Write Mask", node.WriteMask));
        //    data.Add(new AttributeDataGridViewModel("User Write Mask", node.UserWriteMask));

        //    if (node.NodeClass != NodeClass.Variable) return data;

        //    var variableNode = (VariableNode)node.DataLock;
        //    data.Add(new AttributeDataGridViewModel("Value Rank", variableNode.ValueRank));
        //    data.Add(new AttributeDataGridViewModel("Data Type", variableNode.DataType));
        //    data.Add(new AttributeDataGridViewModel("Namespace Index", variableNode.DataType.NamespaceIndex));
        //    data.Add(new AttributeDataGridViewModel("Identifier", variableNode.DataType.Identifier));
        //    data.Add(new AttributeDataGridViewModel("Id Type", variableNode.DataType.IdType));
        //    data.Add(new AttributeDataGridViewModel("Array Dimensions", variableNode.ArrayDimensions));
        //    data.Add(new AttributeDataGridViewModel("Access Level", variableNode.AccessLevel));
        //    data.Add(new AttributeDataGridViewModel("User Access Level", variableNode.UserAccessLevel));
        //    data.Add(new AttributeDataGridViewModel("Historozing", variableNode.Historizing));
        //    data.Add(new AttributeDataGridViewModel("Minimum Sampling", variableNode.MinimumSamplingInterval));
        //    data.Add(new AttributeDataGridViewModel("Value", _uaClientApi.ReadValue(variableNode.NodeId)));
        //    data.Add(new AttributeDataGridViewModel("Data Type", TypeInfo.GetSystemType(variableNode.DataType, new EncodeableFactory())));
        //    data.Add(new AttributeDataGridViewModel("Built In Type", TypeInfo.GetBuiltInType(variableNode.DataType)));

        //    return data;
        //}
        #endregion
    }
}
