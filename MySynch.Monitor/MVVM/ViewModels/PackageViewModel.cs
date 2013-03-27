using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;

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

        private State _packageState;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public State PackageState
        {
            get { return _packageState; }
            set
            {
                if (value != _packageState)
                {
                    _packageState = value;
                    RaisePropertyChanged(() => PackageState);
                }
            }
        }

        private int _totalNumberOfMessages;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public int TotalNumberOfMessages
        {
            get { return _totalNumberOfMessages; }
            set
            {
                if (value != _totalNumberOfMessages)
                {
                    _totalNumberOfMessages = value;
                    RaisePropertyChanged(() => TotalNumberOfMessages);
                }
            }
        }


    }
}
