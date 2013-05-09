using System;

namespace MySynch.Common.IOC
{
    public class ComponentNotRegieteredException:Exception
    {
        public string ComponentName { get; set; }
        public ComponentNotRegieteredException(string componentName, string message):base(message)
        {
            ComponentName = componentName;
        }
    }
}
