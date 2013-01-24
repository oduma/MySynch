using System;
using System.Collections.Generic;
using System.IO;
using Castle.MicroKernel.Registration;
using MySynch.Contracts;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Core.Utilities;
using MySynch.Proxies;

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
            if(installer==null)
                throw new ArgumentNullException("installer");
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
            if (!PublisherAlive(availableChannel))
                return;

            ;
            if (!SubscriberAlive(availableChannel))
                return;
            availableChannel.Status = Status.Ok;
        }

        private bool SubscriberAlive(AvailableChannel availableChannel)
        {
            if (string.IsNullOrEmpty(availableChannel.SubscriberInfo.EndpointName))
            {
                //the publisher has to be local
                var subscriberLocal = _componentResolver.Resolve<IChangeApplyer>(availableChannel.SubscriberInfo.SubScriberName);
                if (!subscriberLocal.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                    {
                        availableChannel.Status = Status.OfflineTemporary;
                        availableChannel.NoOfFailedAttempts++;
                    }
                    else
                        availableChannel.Status = Status.OfflinePermanent;
                    return false;
                }
                return true;
            }
            //the publisher has to be remote
            var subscriberRemote =
                _componentResolver.Resolve<ISubscriberProxy>(availableChannel.SubscriberInfo.SubScriberName,
                                                                availableChannel.SubscriberInfo.EndpointName);
            if (!subscriberRemote.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                }
                else
                    availableChannel.Status = Status.OfflinePermanent;
                return false;
            }
            return true;
        }

        private bool PublisherAlive(AvailableChannel availableChannel)
        {
            if (string.IsNullOrEmpty(availableChannel.PublisherInfo.EndpointName))
            {
                //the publisher has to be local
                var localPublisher = _componentResolver.Resolve<IPublisher>(availableChannel.PublisherInfo.PublisherInstanceName);
                if (!localPublisher.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                    {
                        availableChannel.Status = Status.OfflineTemporary;
                        availableChannel.NoOfFailedAttempts++;
                    }
                    else
                        availableChannel.Status = Status.OfflinePermanent;
                    return false;
                }
                return true;
            }
            //the publisher has to be remote
            var remotePublisher = _componentResolver.Resolve<IPublisherProxy>(availableChannel.PublisherInfo.PublisherInstanceName, 
                                                                                availableChannel.PublisherInfo.EndpointName);
            if (!remotePublisher.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                }
                else
                    availableChannel.Status = Status.OfflinePermanent;
                return false;
            }
            return true;
        }
    }
}
