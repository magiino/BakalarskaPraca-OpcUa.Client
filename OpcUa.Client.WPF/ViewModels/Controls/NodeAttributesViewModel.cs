using System;
using System.Windows.Input;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
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
        public string ValueToWrite { get; set; }
        #endregion

        #region Commands
        public ICommand WriteValueCommand { get; }
        public ICommand ReadValueCommand { get; }
        #endregion

        #region Constructor
        public NodeAttributesViewModel(UaClientApi uaClientApi, Messenger messenger)
        {
            _uaClientApi = uaClientApi;

            WriteValueCommand = new MixRelayCommand(WriteValue);
            ReadValueCommand = new MixRelayCommand(ReadValue);

            messenger.Register<SendSelectedRefNode>(
                msg =>
                {
                    ReferenceDescription = msg.ReferenceNode;
                    UpdateValues(ReferenceDescription);
                });
        }
        #endregion

        #region Command Methods
        private void WriteValue(object parameter)
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, new NamespaceTable());

            var value = _uaClientApi.WriteValue(nodeId, BuiltInType,  ValueToWrite);

            if (value == null)
            {
                IoC.AppManager.ShowWarningMessage("Value failed write to server!");
                return;
            }

            DataValue = value;
        }

        private void ReadValue(object parameter)
        {
            var nodeId = ExpandedNodeId.ToNodeId(ReferenceDescription.NodeId, new NamespaceTable());
            try
            {
                DataValue = _uaClientApi.ReadValue(nodeId);
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
        }
        #endregion

        #region Private Helpers
        private void UpdateValues(ReferenceDescription referenceDescription)
        {
            IsVariableType = false;

            NodeId = referenceDescription.NodeId;
            try
            {
                Node = _uaClientApi.ReadNode(NodeId);
            }
            catch (Exception e)
            {

                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }

            if (Node.NodeClass != NodeClass.Variable) return;

            IsVariableType = true;
            VariableNode = (VariableNode)Node.DataLock;
            DataTypeNodeId = VariableNode.DataType;

            try
            {
                DataValue = _uaClientApi.ReadValue(VariableNode.NodeId);
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }

            DataType = TypeInfo.GetSystemType(VariableNode.DataType, new EncodeableFactory());
            BuiltInType = TypeInfo.GetBuiltInType(VariableNode.DataType);
        }
        #endregion
    }
}