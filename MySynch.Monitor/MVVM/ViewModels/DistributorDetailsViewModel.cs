using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class DistributorDetailsViewModel:ViewModelBase
    {
        public string DistributorName { get; set; }

        private ObservableCollection<PublisherViewModel> _publisherCollection;
        public ObservableCollection<PublisherViewModel> PublisherCollection
        {
            get { return _publisherCollection; }
            set
            {
                if (_publisherCollection != value)
                {
                    _publisherCollection = value;
                    RaisePropertyChanged(() => PublisherCollection);
                }
            }
        }

        public DistributorDetailsViewModel(IDistributorMonitorProxy distributorMonitorProxy)
        {
            var distributorInformation = distributorMonitorProxy.ListAvailableComponentsTree();

            DistributorName = distributorInformation.Name;
            PublisherCollection = new ObservableCollection<PublisherViewModel>();
            foreach (var availablePublisher in distributorInformation.AvailablePublishers)
                PublisherCollection.Add(new PublisherViewModel(availablePublisher));
        }
    }
}
