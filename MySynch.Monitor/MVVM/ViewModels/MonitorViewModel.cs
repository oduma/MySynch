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

        private ObservableCollection<MapChannelViewModel> _availableComponents;

        public ObservableCollection<MapChannelViewModel> AvailableComponents
        {
            get { return _availableComponents; }
            set
            {
                if (_availableComponents != value)
                {
                    _availableComponents = value;
                    RaisePropertyChanged(() => AvailableComponents);
                }
            }
        }


        public MonitorViewModel(IDistributorMonitorProxy distributorMonitorProxy,ListAvailableChannelsResponse listAvailableComponentsTreeResponse)
        {
            _distributorMonitorProxy = distributorMonitorProxy;
            DistributorName = listAvailableComponentsTreeResponse.Name;
            AvailableComponents=new ObservableCollection<MapChannelViewModel>();
            ReadComponents(listAvailableComponentsTreeResponse.Channels);
        }

        public void InitiateView()
        {
            var availableComponents= _distributorMonitorProxy.ListAvailableChannels();
            DistributorName = availableComponents.Name;
            AvailableComponents= new ObservableCollection<MapChannelViewModel>();
            ReadComponents(availableComponents.Channels);


        }

        private void ReadComponents(List<MapChannel> availableComponents)
        {
            foreach(var availableComponent in availableComponents)
                AvailableComponents.Add(new MapChannelViewModel{MapChannelPublisherTitle=availableComponent.PublisherInfo.InstanceName});
        }
    }
}
