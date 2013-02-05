using System.Collections.ObjectModel;
using System.Windows.Media;
using MySynch.Common;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class PublisherViewModel:ViewModelBase
    {
        private ObservableCollection<SubscriberViewModel> _subscriberCollection;
        public ObservableCollection<SubscriberViewModel> SubscriberCollection
        {
            get { return _subscriberCollection; }
            set
            {
                if (_subscriberCollection != value)
                {
                    _subscriberCollection = value;
                    RaisePropertyChanged(() => SubscriberCollection);
                }
            }
        }

        public PublisherViewModel(AvailableComponent availablePublisher)
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                PublisherName = availablePublisher.Name;
                IsLocal = availablePublisher.IsLocal;
                Status = (availablePublisher.Status == Contracts.Messages.Status.Ok) ? Brushes.Green : Brushes.Red;
                SubscriberCollection = new ObservableCollection<SubscriberViewModel>();
                if(availablePublisher.DependentComponents!=null)
                    foreach (var availableSubscriber in availablePublisher.DependentComponents)
                        SubscriberCollection.Add(new SubscriberViewModel(availableSubscriber));
                if (availablePublisher.Packages != null)
                    foreach (var publisherPackage in availablePublisher.Packages)
                        PublisherPackagesCollection.Add(new PackageViewModel(publisherPackage));
            }

        }

        public string PublisherName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }

        
    }
}
