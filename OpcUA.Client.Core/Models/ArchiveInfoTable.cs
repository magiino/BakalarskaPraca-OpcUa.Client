namespace OpcUa.Client.Core
{
    public class ArchiveInfoTable : BaseViewModel
    {
        public ArchiveInterval ArchiveInterval { get; set; }
        public int VariablesCount { get; set; }
        public bool Running { get; set; }
    }
}
