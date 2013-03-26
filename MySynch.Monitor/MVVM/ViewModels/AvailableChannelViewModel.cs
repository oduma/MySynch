using System.Collections.ObjectModel;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class AvailableChannelViewModel:MapChannelViewModel
    {
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

        private ObservableCollection<PackageViewModel> _packagesAtPublisher;

        public ObservableCollection<PackageViewModel> PackagesAtPublisher
        {
            get { return _packagesAtPublisher; }
            set
            {
                if (_packagesAtPublisher != value)
                {
                    _packagesAtPublisher = value;
                    RaisePropertyChanged(() => PackagesAtPublisher);
                }
            }
        }

        private ObservableCollection<PackageViewModel> _packagesAtSubscriber;

        public ObservableCollection<PackageViewModel> PackagesAtSubscriber
        {
            get { return _packagesAtSubscriber; }
            set
            {
                if (_packagesAtSubscriber != value)
                {
                    _packagesAtSubscriber = value;
                    RaisePropertyChanged(() => PackagesAtSubscriber);
                }
            }
        }


    }
}
