using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MySynch.Contracts.Messages;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MonitorViewModel:ViewModelBase
    {
        private IDistributorMonitorProxy _distributorMonitorProxy;

        private string _distributorName;
        public string DistributorName
        {
            get
            {
                return this._distributorName;
            }

            set
            {
                if (value != _distributorName)
                {
                    _distributorName = value;
                    RaisePropertyChanged("DistributorName");
                }
            }
        }

        private ObservableCollection<AvailableChannelViewModel> _availableChannels;

        public ObservableCollection<AvailableChannelViewModel> AvailableChannels
        {
            get { return _availableChannels; }
            set
            {
                if (_availableChannels != value)
                {
                    _availableChannels = value;
                    RaisePropertyChanged(() => AvailableChannels);
                }
            }
        }


        public MonitorViewModel(IDistributorMonitorProxy distributorMonitorProxy,ListAvailableChannelsResponse listAvailableComponentsTreeResponse)
        {
            _distributorMonitorProxy = distributorMonitorProxy;
            DistributorName = listAvailableComponentsTreeResponse.Name;
            AvailableChannels = new ObservableCollection<AvailableChannelViewModel>();
            ReadComponents(listAvailableComponentsTreeResponse.Channels);
        }

        public void InitiateView()
        {
            var availableChannels= _distributorMonitorProxy.ListAvailableChannels();
            DistributorName = availableChannels.Name;
            AvailableChannels = new ObservableCollection<AvailableChannelViewModel>();
            ReadComponents(availableChannels.Channels);


        }

        private void ReadComponents(List<MapChannel> availableChannels)
        {
            foreach(var availableChannel in availableChannels)
                AvailableChannels.Add(new AvailableChannelViewModel { MapChannelPublisherTitle = availableChannel.PublisherInfo.InstanceName, MapChannelSubscriberTitle = availableChannel.SubscriberInfo.InstanceName });
        }
    }
}
