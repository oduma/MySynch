using System.Collections.ObjectModel;
using System.Windows.Media;
using MySynch.Common;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class SubscriberViewModel : ViewModelBase
    {
        public SubscriberViewModel(AvailableComponent availableSubscriber)
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                SubscriberName = availableSubscriber.Name;
                IsLocal = availableSubscriber.IsLocal;
                Status = (availableSubscriber.Status == Contracts.Messages.Status.Ok) ? Brushes.Green : Brushes.Red;
                if(availableSubscriber.Packages!=null)
                    foreach(var package in availableSubscriber.Packages)
                        SubscriberPackagesCollection.Add(new PackageViewModel(package));
            }
        }

        public string SubscriberName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }

        private ObservableCollection<PackageViewModel> _subscriberPackagesCollection;
        public ObservableCollection<PackageViewModel> SubscriberPackagesCollection
        {
            get { return _subscriberPackagesCollection; }
            set
            {
                if (_subscriberPackagesCollection != value)
                {
                    _subscriberPackagesCollection = value;
                    RaisePropertyChanged(() => SubscriberPackagesCollection);
                }
            }
        }


    }
}
