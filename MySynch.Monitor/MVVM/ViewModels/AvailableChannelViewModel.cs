using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class AvailableChannelViewModel:MapChannelViewModel
    {
        public int NoOfTimesPresentedDone
        {
            get;
            set;
        }

        private Visibility _messagesVisible;
        public Visibility MessagesVisible
        {
            get
            {
                return this._messagesVisible;
            }

            set
            {
                if (value != _messagesVisible)
                {
                    _messagesVisible = value;
                    RaisePropertyChanged(()=>MessagesVisible);
                }
            }
        }

        private Status _publisherStatus;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public Status PublisherStatus
        {
            get { return _publisherStatus; }
            set
            {
                if (value != _publisherStatus)
                {
                    _publisherStatus = value;
                    RaisePropertyChanged(() => PublisherStatus);
                }
            }
        }

        private Status _subscriberStatus;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public Status SubscriberStatus
        {
            get { return _subscriberStatus; }
            set
            {
                if (value != _subscriberStatus)
                {
                    _subscriberStatus = value;
                    RaisePropertyChanged(() => SubscriberStatus);
                }
            }
        }

        private string _publisherRootPath;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public string PublisherRootPath
        {
            get { return _publisherRootPath; }
            set
            {
                if (value != _publisherRootPath)
                {
                    _publisherRootPath = value;
                    RaisePropertyChanged(() => PublisherRootPath);
                }
            }
        }

        private string _subscriberRootPath;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public string SubscriberRootPath
        {
            get { return _subscriberRootPath; }
            set
            {
                if (value != _subscriberRootPath)
                {
                    _subscriberRootPath = value;
                    RaisePropertyChanged(() => SubscriberRootPath);
                }
            }
        }

        private State _publisherPackageState;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public State PublisherPackageState
        {
            get { return _publisherPackageState; }
            set
            {
                if (value != _publisherPackageState)
                {
                    _publisherPackageState = value;
                    RaisePropertyChanged(() => PublisherPackageState);
                }
            }
        }



        private ObservableCollection<MessageViewModel> _messagesProcessed;

        public ObservableCollection<MessageViewModel> MessagesProcessed
        {
            get { return _messagesProcessed; }
            set
            {
                if (_messagesProcessed != value)
                {
                    _messagesProcessed = value;
                    RaisePropertyChanged(() => MessagesProcessed);
                }
            }
        }

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

        private State _subscriberPackageState;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public State SubscriberPackageState
        {
            get { return _subscriberPackageState; }
            set
            {
                if (value != _subscriberPackageState)
                {
                    _subscriberPackageState = value;
                    RaisePropertyChanged(() => SubscriberPackageState);
                }
            }
        }



    }
}
