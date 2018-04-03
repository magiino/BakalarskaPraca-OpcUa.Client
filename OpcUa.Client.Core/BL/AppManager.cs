using System;

namespace OpcUa.Client.Core
{
    public class AppManager
    {
        public int? ProjectId { get; set; }
        public bool IsSaved { get; set; }
        public Action CloseAction { get; set; }

        public void CloseApplication()
        {
            IoC.DisposeAll();
            CloseAction();
        }
    }
}
