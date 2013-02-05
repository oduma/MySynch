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

        protected string PackageId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        protected State State
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
