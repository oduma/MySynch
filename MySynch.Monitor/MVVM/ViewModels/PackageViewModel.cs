using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class PackageViewModel:ViewModelBase
    {
        private Guid _packageId;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public Guid PackageId
        {
            get { return _packageId; }
            set
            {
                if (value != _packageId)
                {
                    _packageId = value;
                    RaisePropertyChanged(() => PackageId);
                }
            }
        }


    }
}
