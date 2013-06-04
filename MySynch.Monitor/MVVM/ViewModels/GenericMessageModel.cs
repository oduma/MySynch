using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class GenericMessageModel
    {
        public string Message { get; set; }

        public ComponentType Source { get; set; }
    }
}
