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

    }
}
