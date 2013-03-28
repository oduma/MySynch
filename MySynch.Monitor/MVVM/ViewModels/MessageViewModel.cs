using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MessageViewModel:ViewModelBase
    {
        private string _fullTargetPath;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public string FullTargetPath
        {
            get { return _fullTargetPath; }
            set
            {
                if (value != _fullTargetPath)
                {
                    _fullTargetPath = value;
                    RaisePropertyChanged(() => FullTargetPath);
                }
            }
        }

        private OperationType _operationType;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public OperationType OperationType
        {
            get { return _operationType; }
            set
            {
                if (value != _operationType)
                {
                    _operationType = value;
                    RaisePropertyChanged(() => OperationType);
                }
            }
        }

        private bool _done;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool Done
        {
            get { return _done; }
            set
            {
                if (value != _done)
                {
                    _done = value;
                    RaisePropertyChanged(() => Done);
                }
            }
        }


    }
}
