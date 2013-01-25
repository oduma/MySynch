using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
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
            
            if(AvailableChannels.FirstOrDefault(c=>c.Status!=Status.Ok)!=null)
                return;
            //the channels have to be processed in the order of the publisher
            var publisherGroups =
                AvailableChannels.GroupBy(
                    c => c.PublisherInfo.PublisherInstanceName + "-" + c.PublisherInfo.EndpointName);
            foreach (var publisherGroup in publisherGroups)
            {
                var currentPublisher = publisherGroup.Select(g => g).First().PublisherInfo.Publisher;
                var packagePublished = currentPublisher.PublishPackage();
                if(packagePublished==null)
                    continue;
                if (DistributeMessages(publisherGroup.Select(g => g), packagePublished))
                    //Publisher's messages not needed anymore
                    currentPublisher.RemovePackage(packagePublished);
            }
        }

        private bool DistributeMessages(IEnumerable<AvailableChannel> channelsFromAPublisher,ChangePushPackage package)
        {
            bool result = true;
            foreach (var channel in channelsFromAPublisher)
            {
                CheckChannel(channel);
                if (channel.Status != Status.Ok)
                    return false;

                if (!channel.SubscriberInfo.Subscriber.ApplyChangePackage(package, channel.SubscriberInfo.TargetRootFolder,
                                                                     channel.CopyStrategy.Copy))
                    result = false;
            }
            return result;
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
            //Initialize all the active channels
            AvailableChannels.Where(c=>c.Status==Status.Ok).ForEach(InitializeChannel);
        }

        private void InitializeChannel(AvailableChannel channel)
        {
            //publisher
            if (string.IsNullOrEmpty(channel.PublisherInfo.EndpointName))
            {
                //publisher has to be local
                channel.PublisherInfo.Publisher =
                    _componentResolver.Resolve<IPublisher>(channel.PublisherInfo.PublisherInstanceName);
            }
            else
            {
                //publisher has to be local
                channel.PublisherInfo.Publisher =
                    _componentResolver.Resolve<IPublisherProxy>(channel.PublisherInfo.PublisherInstanceName,
                                                                channel.PublisherInfo.EndpointName);

            }
            //subscriber
            if (string.IsNullOrEmpty(channel.PublisherInfo.EndpointName))
            {
                //subscriber has to be local
                channel.SubscriberInfo.Subscriber =
                    _componentResolver.Resolve<IChangeApplyer>(channel.SubscriberInfo.SubScriberName);
            }
            else
            {
                //subscriber has to be remote
                channel.SubscriberInfo.Subscriber =
                    _componentResolver.Resolve<ISubscriberProxy>(channel.SubscriberInfo.SubScriberName,
                                                                channel.SubscriberInfo.EndpointName);

            }
            //copy strategy
            channel.CopyStrategy = _componentResolver.Resolve<ICopyStrategy>(channel.CopyStartegyName);
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
