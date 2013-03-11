using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using MySynch.Common.Serialization;
using MySynch.Core.DataTypes;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MapEditorViewModel:ViewModelBase
    {
        private string _distributorMapFile;
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


        public MapEditorViewModel()
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "DistributorMap");
            if (key == null)
                _distributorMapFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"map\distributormap.xml");
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(_distributorMapFile))
                return;
            if (!File.Exists(_distributorMapFile))
                return;
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
            foreach(var mapChannel in mapChannels)
            {
                MapChannels.Add(mapChannel);
                MapChannelViewModel channel = mapChannel;
                if(!AllAvailablePublishers.Any(p=>p==channel.MapChannelPublisherTitle))
                {
                    AllAvailablePublishers.Add(mapChannel.MapChannelPublisherTitle);
                }
                if(!AllAvailableSubscribers.Any(s=>s==mapChannel.MapChannelSubscriberTitle))
                {
                    AllAvailableSubscribers.Add(mapChannel.MapChannelSubscriberTitle);
                }
            }
            
        }
    }
}
