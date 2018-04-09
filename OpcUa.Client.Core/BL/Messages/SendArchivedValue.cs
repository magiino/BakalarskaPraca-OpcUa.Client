namespace OpcUa.Client.Core
{
    public class SendManageArchivedValue
    {
        public bool Delete { get; set; }

        public VariableEntity Variable { get; set; }

        public SendManageArchivedValue(bool delete, VariableEntity variable)
        {
            Delete = delete;
            Variable = variable;
        }
    }
}
