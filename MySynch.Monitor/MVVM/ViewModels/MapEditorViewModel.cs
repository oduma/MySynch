using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Windows;
using System.Windows.Input;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Core.DataTypes;
using MySynch.Core.WCF.Clients.Discovery;
using MySynch.Monitor.Utils;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MapEditorViewModel:AsynchViewModelBase
    {
        private const string PlaceHolderLoading = "Still loading...";
        private string _distributorMapFile;
        private object _publisherLock = new object();
        private object _subscriberLock = new object();

        private ObservableCollection<MapChannelViewModel> _mapChannels;

        public ObservableCollection<MapChannelViewModel> MapChannels
        {
            get { return _mapChannels; }
            set
            {
                if (_mapChannels != value)
                {
                    _mapChannels = value;
                    RaisePropertyChanged(() => MapChannels);
                }
            }
        }

        private ObservableCollection<string> _allAvailablePublishers;

        public ObservableCollection<string> AllAvailablePublishers
        {
            get { return _allAvailablePublishers; }
            set
            {
                if (_allAvailablePublishers != value)
                {
                    _allAvailablePublishers = value;
                    RaisePropertyChanged(() => AllAvailablePublishers);
                }
            }
        }

        private ObservableCollection<string> _allAvailableSubscribers;

        public ObservableCollection<string> AllAvailableSubscribers
        {
            get { return _allAvailableSubscribers; }
            set
            {
                if (_allAvailableSubscribers != value)
                {
                    _allAvailableSubscribers = value;
                    RaisePropertyChanged(() => AllAvailableSubscribers);
                }
            }
        }

        public ICommand SaveAndRestart { get; private set; }

        public ICommand Close { get; private set; }
        
        private bool _closeOnlyWindow;

        public MapEditorViewModel(bool closeOnlyWindow = false)
        {
            _closeOnlyWindow = closeOnlyWindow;
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "DistributorMap");
            if (key == null)
                _distributorMapFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"map\distributormap.xml");
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(_distributorMapFile))
                return;
            if (!File.Exists(_distributorMapFile))
                return;
        }

        public void InitiateView(bool searchNetwork=true)
        {
            var mapChannels =
                Serializer.DeserializeFromFile<AvailableChannel>(_distributorMapFile).Select(
                    c =>
                    new MapChannelViewModel
                        {
                            MapChannelPublisherTitle = c.PublisherInfo.InstanceName + ":" + c.PublisherInfo.Port,
                            MapChannelSubscriberTitle = c.SubscriberInfo.InstanceName + ":" + c.SubscriberInfo.Port
                        });
            MapChannels=new ObservableCollection<MapChannelViewModel>();
            AllAvailablePublishers=new ObservableCollection<string>();
            AllAvailableSubscribers=new ObservableCollection<string>();
            SaveAndRestart = new RelayCommand(PerformSaveAnRestart);
            Close = new RelayCommand(PerformClose);
            if (searchNetwork)
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
                backgroundWorker.ProgressChanged += ProgressChanged;
                backgroundWorker.RunWorkerAsync();
            }
            foreach(var mapChannel in mapChannels)
            {
                MapChannels.Add(mapChannel);
                MapChannelViewModel channel = mapChannel;
                lock (_publisherLock)
                {
                    if (!AllAvailablePublishers.Any(p => p == channel.MapChannelPublisherTitle))
                    {
                        AllAvailablePublishers.Add(mapChannel.MapChannelPublisherTitle);
                    }
                }
                lock(_subscriberLock)
                {
                    if(!AllAvailableSubscribers.Any(s=>s==channel.MapChannelSubscriberTitle))
                    {
                        AllAvailableSubscribers.Add(mapChannel.MapChannelSubscriberTitle);
                    }
                }
            }
            if (searchNetwork)
            {
                lock(_publisherLock)
                {
                    AllAvailablePublishers.Add(PlaceHolderLoading);
                }
                lock(_subscriberLock)
                {
                    AllAvailableSubscribers.Add(PlaceHolderLoading);
                }
            }
        }

        private void PerformClose()
        {
            if(!_closeOnlyWindow)
                Application.Current.Shutdown();
            else
            {
                if (RequestHide != null)
                    RequestHide(this, null);
            }
        }

        internal void PerformSaveAnRestart()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoSaveWork;
            backgroundWorker.RunWorkerCompleted += DoSaveWorkCompleted;
            backgroundWorker.ProgressChanged += DoSaveWorkProgressChanged;
            backgroundWorker.RunWorkerAsync();

            BlockTheUI(DoSaveWorkProgressChanged);
        }


        private void DoSaveWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkingMessage = e.UserState.ToString();
            return;
        }

        private void DoSaveWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartDistributor();
            UnblockTheUI();
        }

        private void DoSaveWork(object sender, DoWorkEventArgs e)
        {
            StopDistributor();
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Saving changes."));
            Serializer.SerializeToFile(MapChannels.ConvertToChannels().Distinct(new ChannelEqualityComparer()).ToList(), _distributorMapFile);
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Changes saved."));
        }

        internal virtual void StartDistributor()
        {
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Starting the Distributor."));
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, ServiceHelper.StartDistributor()));
            return;
        }

        internal virtual void StopDistributor()
        {
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Stopping the Distributor."));
            DoSaveWorkProgressChanged(this, new ProgressChangedEventArgs(0, ServiceHelper.StopDistributor()));
            return;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            return;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock(_publisherLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => AllAvailablePublishers.Remove(PlaceHolderLoading)));
            }
            lock(_subscriberLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => AllAvailableSubscribers.Remove(PlaceHolderLoading)));
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            GetAllPublishers();
            GetAllSubscribers();
        }

        private void GetAllSubscribers()
        {
            var subscribers = DiscoveryHelper.FindServices<ISubscriber>();
            lock (_subscriberLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => LoadSubscriberInformation(subscribers)));
            }
        }

        private void LoadSubscriberInformation(IEnumerable<EndpointDiscoveryMetadata> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                var newSubscriber = "ISubscriber.Remote:" + subscriber.Address.Uri.Port;
                if (!AllAvailableSubscribers.Any(s => s == newSubscriber))
                {
                    AllAvailableSubscribers.Remove(PlaceHolderLoading);
                    AllAvailableSubscribers.Add(newSubscriber);
                    AllAvailableSubscribers.Add(PlaceHolderLoading);
                }
            }
        }

        private void GetAllPublishers()
        {
            var publishers = DiscoveryHelper.FindServices<IPublisher>();

            lock (_publisherLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => LoadPublisherInformation(publishers)));
            }
        }

        private void LoadPublisherInformation(IEnumerable<EndpointDiscoveryMetadata> publishers)
        {

            foreach (var publisher in publishers)
            {
                var newPublisher = "IPublisher.Remote:" + publisher.Address.Uri.Port;

                if (!AllAvailablePublishers.Any(p => p == newPublisher))
                {
                    AllAvailablePublishers.Remove(PlaceHolderLoading);
                    AllAvailablePublishers.Add(newPublisher);
                    AllAvailablePublishers.Add(PlaceHolderLoading);
                }
            }
        }

        public event Action<object, object> RequestHide;
    }
}
