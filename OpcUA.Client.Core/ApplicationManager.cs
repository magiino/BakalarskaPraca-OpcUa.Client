namespace OpcUA.Client.Core
{
    public class ApplicationManager
    {
        private UaClientApi _uaClient;

        public ApplicationManager(UaClientApi uaClient)
        {
            _uaClient = uaClient;
        }
    }
}
