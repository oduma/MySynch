using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MonitorDetailViewModel:ViewModelBase
    {
        private string _relativePath;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                if (value == _relativePath) return;
                _relativePath = value;
                RaisePropertyChanged(() => RelativePath);
            }
        }

    }
}
