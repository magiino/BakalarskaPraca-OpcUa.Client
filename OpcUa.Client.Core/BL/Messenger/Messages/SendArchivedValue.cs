namespace OpcUa.Client.Core
{
    public class SendManageArchivedValue
    {
        public bool Delete { get; set; }

        public VariableModel Variable { get; set; }

        public SendManageArchivedValue(bool delete, VariableModel variable)
        {
            Delete = delete;
            Variable = variable;
        }
    }
}