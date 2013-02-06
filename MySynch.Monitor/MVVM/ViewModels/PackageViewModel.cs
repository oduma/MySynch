using System;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class PackageViewModel:ViewModelBase
    {
        public PackageViewModel(Package package)
        {
            PackageId = package.Id.ToString();

            State = package.State;
        }

        public string PackageId { get; set; }

        public State State { get; set; }
    }
}
