using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Common.Serialization;
using MySynch.Core.DataTypes;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MapEditorViewModel:ViewModelBase
    {
        private ObservableCollection<MapChannelViewModel> _mapChannels;
        private string _distributorMapFile;

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
            foreach(var mapChannel in mapChannels)
                MapChannels.Add(mapChannel);
        }
    }
}
