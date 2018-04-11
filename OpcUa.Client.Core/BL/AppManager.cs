using System;

namespace OpcUa.Client.Core
{
    public class AppManager
    {
        public Guid ProjectId { get; set; }
        public bool IsSaved { get; set; }
        public Action CloseAction { get; set; }

        public void CloseApplication()
        {
            IoC.DisposeAll();
            CloseAction();
        }

        // TODO z tadeto vypisovat okná, aspon message dialogy
    }
}