using System;
using System.Collections.Generic;
using System.IO;
using Castle.MicroKernel.Registration;
using MySynch.Contracts;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Core.Utilities;

namespace MySynch.Core
{
    public class Distributor:IDistributor
    {
        private ComponentResolver _componentResolver = new ComponentResolver();
        private readonly int _maxNoOfFailedAttempts = 3;

        public List<AvailableChannel> AvailableChannels { get; private set; }

        public void DistributeMessages()
        {
            //Recheck the AvailableChannels
            AvailableChannels.ForEach(CheckChannel);
        }

        public void InitiateDistributionMap(string mapFile,IWindsorInstaller installer)
        {
            _componentResolver.RegisterAll(installer);
            if(string.IsNullOrEmpty(mapFile))
                throw new ArgumentNullException();
            if(!File.Exists(mapFile))
                throw new ArgumentException();
            AvailableChannels = Serializer.DeserializeFromFile<AvailableChannel>(mapFile);
            if(AvailableChannels==null || AvailableChannels.Count<=0)
                throw new Exception("No channels in the map file");
            AvailableChannels.ForEach(CheckChannel);
        }

        private void CheckChannel(AvailableChannel availableChannel)
        {
            var publisher = _componentResolver.Resolve<IPublisher>(availableChannel.PublisherInfo.PublisherInstanceName);
            if(!publisher.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                }
                else
                    availableChannel.Status = Status.OfflinePermanent;
                return;
            }
            var subscriber = _componentResolver.Resolve<IChangeApplyer>(availableChannel.SubscriberInfo.SubScriberName);
            if (!subscriber.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                }
                else
                    availableChannel.Status = Status.OfflinePermanent;
                return;
            }
            availableChannel.Status = Status.Ok;
        }
    }
}
