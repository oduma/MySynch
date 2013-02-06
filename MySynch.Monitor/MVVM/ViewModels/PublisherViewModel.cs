using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
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

        private ObservableCollection<PackageViewModel> _publisherPackagesCollection;
        public ObservableCollection<PackageViewModel> PublisherPackagesCollection
        {
            get { return _publisherPackagesCollection; }
            set
            {
                if (_publisherPackagesCollection != value)
                {
                    _publisherPackagesCollection = value;
                    RaisePropertyChanged(() => PublisherPackagesCollection);
                }
            }
        }

        public CompositeCollection Children
        {
            get
            {
                return new CompositeCollection
                           {
                               new CollectionContainer() {Collection = PublisherPackagesCollection},
                               new CollectionContainer() {Collection = SubscriberCollection}
                           };
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
                //if (availablePublisher.Packages != null)
                //    foreach (var publisherPackage in availablePublisher.Packages)
                //        PublisherPackagesCollection.Add(new PackageViewModel(publisherPackage));
                PublisherPackagesCollection= new ObservableCollection<PackageViewModel>();
                PublisherPackagesCollection.Add(new PackageViewModel(new Package{Id=Guid.NewGuid(),State=State.Published}));
                PublisherPackagesCollection.Add(new PackageViewModel(new Package { Id = Guid.NewGuid(), State = State.Published }));
            }

        }

        public string PublisherName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }

        
    }
}
