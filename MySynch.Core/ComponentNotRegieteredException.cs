using System;

namespace MySynch.Core
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
