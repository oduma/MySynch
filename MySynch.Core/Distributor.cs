﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Castle.Core.Internal;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies;

namespace MySynch.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Distributor:IDistributor
    {
        private ComponentResolver _componentResolver = new ComponentResolver();
        private readonly int _maxNoOfFailedAttempts = 3;
        private List<AvailableComponent> _allComponents;

        public List<AvailableChannel> AvailableChannels { get; private set; }

        public void DistributeMessages()
        {
            try
            {
                LoggingManager.Debug("Starting to distribute messages ...");
                //Recheck the AvailableChannels
                AvailableChannels.ForEach(CheckChannel);

                var availableChannels = AvailableChannels.Where(c => c.Status == Status.Ok);
                if (availableChannels == null || availableChannels.Count() == 0)
                    return;
                //the channels have to be processed in the order of the publisher
                var publisherGroups =
                    availableChannels.GroupBy(
                        c => c.PublisherInfo.PublisherInstanceName + "-" + c.PublisherInfo.EndpointName);
                foreach (var publisherGroup in publisherGroups)
                {
                    var publisherChannelsNotAvailable =
                        AvailableChannels.Count(
                            c =>
                            ((c.PublisherInfo.PublisherInstanceName + "-" + c.PublisherInfo.EndpointName) ==
                             publisherGroup.Key) && c.Status != Status.Ok);

                    var currentPublisher = publisherGroup.Select(g => g).First().PublisherInfo.Publisher;

                    var packagePublished = currentPublisher.PublishPackage();

                    if (packagePublished == null)
                        continue;

                    if (DistributeMessages(publisherGroup.Select(g => g), packagePublished) && publisherChannelsNotAvailable == 0)
                        //Publisher's messages not needed anymore
                        currentPublisher.RemovePackage(packagePublished);
                }

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
            LoggingManager.Debug("Dsitributed all available messages.");
        }

        private bool DistributeMessages(IEnumerable<AvailableChannel> channelsFromAPublisher,ChangePushPackage package)
        {
            LoggingManager.Debug("Distribute messages from package:" + package.PackageId);
            bool result = true;
            foreach (var channel in channelsFromAPublisher)
            {
                CheckChannel(channel);
                if (channel.Status != Status.Ok)
                {
                    result = false;
                    continue;
                }
                try
                {
                    if (!channel.SubscriberInfo.Subscriber.ApplyChangePackage(package, channel.SubscriberInfo.TargetRootFolder,
                                                                         channel.CopyStrategy.Copy))
                        result = false;
                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError("Error while applying the changes", ex);
                    result = false;                    
                }
            }
            LoggingManager.Debug("Message distributed: " +package.PackageId);
            return result;
        }

        public void InitiateDistributionMap(string mapFile,ComponentResolver componentResolver)
        {
            if(componentResolver==null)
                throw new ArgumentNullException("componentResolver");

            _componentResolver = componentResolver;
           
            if(string.IsNullOrEmpty(mapFile))
                throw new ArgumentNullException();
            if(!File.Exists(mapFile))
                throw new ArgumentException();
            AvailableChannels = Serializer.DeserializeFromFile<AvailableChannel>(mapFile);
            _allComponents=new List<AvailableComponent>();

            if(AvailableChannels==null || AvailableChannels.Count<=0)
                throw new Exception("No channels in the map file");
            try
            {
                LoggingManager.Debug("Initiating distributor with map: " + mapFile);
                AvailableChannels.ForEach(CheckChannel);
                //Initialize all the active channels
                AvailableChannels.Where(c => c.Status == Status.Ok).ForEach(c=>c.CopyStrategy = _componentResolver.Resolve<ICopyStrategy>(c.CopyStartegyName));
                LoggingManager.Debug("Initiated Ok.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }


        //[TODO] Check if it still needed
        private static void CheckRegistration(ComponentResolver componentResolver)
        {
            componentResolver.ResolveAll<IPublisher>();
            componentResolver.ResolveAll<IPublisherProxy>();
            componentResolver.ResolveAll<ICopyStrategy>();
            componentResolver.ResolveAll<IChangeApplyer>();
            componentResolver.ResolveAll<ISubscriberProxy>();

        }

        private void CheckChannel(AvailableChannel availableChannel)
        {
            if (!PublisherAlive(availableChannel))
                return;
            if (!SubscriberAlive(availableChannel))
                return;
            availableChannel.Status = Status.Ok;
        }

        private bool SubscriberAlive(AvailableChannel availableChannel)
        {
            var publisherName = string.Format("{0}{1}",
                                              availableChannel.PublisherInfo.
                                                  PublisherInstanceName,
                                              (string.IsNullOrEmpty(
                                                  availableChannel.PublisherInfo.
                                                      EndpointName))
                                                  ? ""
                                                  : "." +
                                                    availableChannel.PublisherInfo.
                                                        EndpointName);
            AvailableComponent availableComponent = new AvailableComponent
            {
                Name =
                    string.Format("{0}{1}",
                                  availableChannel.SubscriberInfo.
                                      SubScriberName,
                                  (string.IsNullOrEmpty(
                                      availableChannel.SubscriberInfo.
                                          EndpointName))
                                      ? ""
                                      : "." +
                                        availableChannel.SubscriberInfo.
                                            EndpointName),
                Status = Status.NotChecked
            };


            if (string.IsNullOrEmpty(availableChannel.SubscriberInfo.EndpointName))
            {
                IChangeApplyer subscriberLocal;
                //the publisher has to be local
                try
                {
                 subscriberLocal = (availableChannel.SubscriberInfo.Subscriber) ??
                                      (availableChannel.SubscriberInfo.Subscriber =
                                       _componentResolver.Resolve<IChangeApplyer>(
                                           availableChannel.SubscriberInfo.SubScriberName));

                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError(availableChannel.SubscriberInfo.SubScriberName,ex);
                    availableChannel.Status = Status.OfflinePermanent;
                    availableComponent.Status = Status.OfflinePermanent;
                    return false;
                }

                if (!subscriberLocal.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                    {
                        availableChannel.Status = Status.OfflineTemporary;
                        availableChannel.NoOfFailedAttempts++;
                        availableComponent.Status = Status.OfflineTemporary;
                    }
                    else
                    {
                        availableChannel.Status = Status.OfflinePermanent;
                        availableComponent.Status = Status.OfflinePermanent;
                    }
                    UpsertComponent(availableComponent,publisherName);
                    return false;
                }
                availableComponent.Status = Status.Ok;
                UpsertComponent(availableComponent,publisherName);
                return true;
            }

            IChangeApplyer subscriberRemote;
            //the publisher has to be local
            try
            {
                subscriberRemote = (availableChannel.SubscriberInfo.Subscriber) ??
                                     (availableChannel.SubscriberInfo.Subscriber =
                                      _componentResolver.Resolve<ISubscriberProxy>(
                                          availableChannel.SubscriberInfo.SubScriberName, availableChannel.SubscriberInfo.EndpointName));

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(availableChannel.SubscriberInfo.SubScriberName + availableChannel.SubscriberInfo.EndpointName, ex);
                availableChannel.Status = Status.OfflinePermanent;
                availableComponent.Status = Status.OfflinePermanent;
                return false;
            }
            if (!subscriberRemote.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                    availableComponent.Status = Status.OfflineTemporary;
                }
                else
                {
                    availableChannel.Status = Status.OfflinePermanent;
                    availableComponent.Status = Status.OfflinePermanent;

                }
                UpsertComponent(availableComponent, publisherName);
                return false;
            }
            availableComponent.Status = Status.Ok;
            UpsertComponent(availableComponent, publisherName);
            return true;
        }

        private void UpsertComponent(AvailableComponent availableComponent, string publisherName)
        {
            LoggingManager.Debug("Upserting component for publisher: "+publisherName);
            var existingPublisher = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingPublisher == null)
            {
                LoggingManager.Debug("Publisher not found: " +publisherName);
                return;
            }
            if(existingPublisher.DependentComponents==null)
                existingPublisher.DependentComponents=new List<AvailableComponent>();
            var existingComponent =
                existingPublisher.DependentComponents.FirstOrDefault(c => c.Name == availableComponent.Name);
            if(existingComponent==null)
                existingPublisher.DependentComponents.Add(availableComponent);
            else
            {
                existingComponent.Status = availableComponent.Status;
                existingComponent.IsLocal = availableComponent.IsLocal;
            }
            LoggingManager.Debug("Components upserted.");
        }

        private bool PublisherAlive(AvailableChannel availableChannel)
        {
            AvailableComponent availableComponent=new AvailableComponent
                                                      {
                                                          Name =
                                                              string.Format("{0}{1}",
                                                                            availableChannel.PublisherInfo.
                                                                                PublisherInstanceName,
                                                                            (string.IsNullOrEmpty(
                                                                                availableChannel.PublisherInfo.
                                                                                    EndpointName))
                                                                                ? ""
                                                                                : "." +
                                                                                  availableChannel.PublisherInfo.
                                                                                      EndpointName),
                                                          Status = Status.NotChecked
                                                      };

            if (string.IsNullOrEmpty(availableChannel.PublisherInfo.EndpointName))
            {
                availableComponent.IsLocal = true;
                IPublisher localPublisher;
                //the publisher has to be local
                try
                {
                    localPublisher = (availableChannel.PublisherInfo.Publisher) ??
                                         (availableChannel.PublisherInfo.Publisher =
                                          _componentResolver.Resolve<IPublisher>(
                                              availableChannel.PublisherInfo.PublisherInstanceName));

                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError(availableChannel.PublisherInfo.PublisherInstanceName, ex);
                    availableChannel.Status = Status.OfflinePermanent;
                    availableComponent.Status = Status.OfflinePermanent;
                    return false;
                }

                //the publisher has to be local
                if (!localPublisher.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                    {
                        availableChannel.Status = Status.OfflineTemporary;
                        availableChannel.NoOfFailedAttempts++;
                        availableComponent.Status = Status.OfflineTemporary;
                    }
                    else
                    {
                        availableChannel.Status = Status.OfflinePermanent;
                        availableComponent.Status = Status.OfflinePermanent;
                    }
                    UpsertComponent(availableComponent);
                    return false;
                }
                availableComponent.Status = Status.Ok;
                UpsertComponent(availableComponent);
                return true;
            }
            //the publisher has to be remote
            availableComponent.IsLocal = false;

            IPublisher remotePublisher;
            //the publisher has to be local
            try
            {
                remotePublisher = (availableChannel.PublisherInfo.Publisher) ??
                                     (availableChannel.PublisherInfo.Publisher =
                                      _componentResolver.Resolve<IPublisherProxy>(
                                          availableChannel.PublisherInfo.PublisherInstanceName, availableChannel.PublisherInfo.EndpointName));

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(availableChannel.PublisherInfo.PublisherInstanceName + availableChannel.PublisherInfo.EndpointName, ex);
                availableChannel.Status = Status.OfflinePermanent;
                availableComponent.Status = Status.OfflinePermanent;
                return false;
            }

            if (!remotePublisher.GetHeartbeat().Status)
            {
                if (availableChannel.NoOfFailedAttempts < _maxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                    availableComponent.Status = Status.OfflineTemporary;
                }
                else
                {
                    availableComponent.Status = Status.OfflinePermanent;
                    availableChannel.Status = Status.OfflinePermanent;
                }
                UpsertComponent(availableComponent);
                return false;
            }
            availableComponent.Status = Status.Ok;
            UpsertComponent(availableComponent);
            return true;
        }

        private void UpsertComponent(AvailableComponent availableComponent)
        {
            LoggingManager.Debug("Upserting publisher:" + availableComponent.Name);
            var existingComponent = _allComponents.FirstOrDefault(p => p.Name == availableComponent.Name);
            if(existingComponent==null)
                _allComponents.Add(availableComponent);
            else
            {
                existingComponent.Status = availableComponent.Status;
            }
            LoggingManager.Debug("Publisher upserted.");
        }

        public DistributorComponent ListAvailableComponentsTree()
        {
            LoggingManager.Debug("Listing all available components");
            return new DistributorComponent {Name = Environment.MachineName, AvailablePublishers = _allComponents};
        }

        public HeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new HeartbeatResponse {Status = true};
        }
    }
}
