﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Core.Distributor
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Distributor : IDistributorMonitor
    {
        private ComponentResolver _componentResolver = new ComponentResolver();
        private const int MaxNoOfFailedAttempts = 3;
        private List<AvailableComponent> _allComponents;
        public List<AvailableChannel> AvailableChannels { get; private set; }

        public void DistributeMessages()
        {
            try
            {
                LoggingManager.Debug("Starting to distribute messages ...");
                //For test only keep old trace
                UnRegisterOldPackages(_allComponents);
                //Recheck the AvailableChannels
                AvailableChannels.ForEach(CheckChannel);

                var availableChannels = AvailableChannels.Where(c => c.Status == Status.Ok);
                if (availableChannels.Count() == 0)
                    return;
                //the channels have to be processed in the order of the publisher
                var publisherGroups =
                    availableChannels.GroupBy(
                        c => c.PublisherInfo.PublisherInstanceName + "-" + c.PublisherInfo.EndpointName);
                foreach (var publisherGroup in publisherGroups)
                {
                    IGrouping<string, AvailableChannel> @group = publisherGroup;
                    var publisherChannelsNotAvailable =
                        AvailableChannels.Count(
                            c =>
                            ((c.PublisherInfo.PublisherInstanceName + "-" + c.PublisherInfo.EndpointName) ==
                             group.Key) && c.Status != Status.Ok);

                    var currentPublisher = publisherGroup.Select(g => g).First().PublisherInfo.Publisher;

                    var packagePublished = currentPublisher.PublishPackage();

                    if (packagePublished == null)
                        continue;
                    RegisterPublisherPackage(publisherGroup.Select(g => g).First(), packagePublished, State.Published);

                    if (DistributeMessages(publisherGroup.Select(g => g), packagePublished) &&
                        publisherChannelsNotAvailable == 0)
                        //Publisher's messages not needed anymore
                    {
                        RegisterPublisherPackage(publisherGroup.Select(g => g).First(), packagePublished,
                                                 State.Distributed);
                        currentPublisher.RemovePackage(packagePublished);
                        RegisterPublisherPackage(publisherGroup.Select(g => g).First(), packagePublished,
                                                 State.Removed);
                        foreach (var channel in publisherGroup.Select(g => g))
                            RegisterSubscriberPackage(channel, packagePublished, State.Removed);
                        LoggingManager.Debug("Dsitributed all available messages.");
                    }
                    else
                        LoggingManager.Debug("Some messages were not distributed.");
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }

        private void RegisterPublisherPackage(AvailableChannel channel, PublishPackageRequestResponse packageRequestResponse, State state)
        {
            var publisherName = string.Format("{0}{1}",
                                              channel.PublisherInfo.
                                                  PublisherInstanceName,
                                              (string.IsNullOrEmpty(
                                                  channel.PublisherInfo.
                                                      EndpointName))
                                                  ? ""
                                                  : "." +
                                                    channel.PublisherInfo.
                                                        EndpointName);
            LoggingManager.Debug("Register package: " + packageRequestResponse.PackageId + " with publisher: " + publisherName);

            var existingcomponent = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingcomponent == null)
            {
                LoggingManager.Debug("Publisher doesn't exist:" + publisherName);
                return;
            }
            if (existingcomponent.Packages == null)
                existingcomponent.Packages = new List<Package>();
            var existingPackage = existingcomponent.Packages.FirstOrDefault(p => p.Id == packageRequestResponse.PackageId);
            if (existingPackage != null)
            {
                existingPackage.State = state;
                LoggingManager.Debug("Package exists updating state to: " + state);
                return;
            }
            existingcomponent.Packages.Add(new Package
                                               {
                                                   Id = packageRequestResponse.PackageId,
                                                   State = state,
                                                   PackageMessages = packageRequestResponse.ChangePushItems
                                               });
            LoggingManager.Debug("Registered new package: " + packageRequestResponse.PackageId + " on publisher: " + publisherName +
                                 " with state: " + state);
        }

        private static void UnRegisterOldPackages(IEnumerable<AvailableComponent> components)
        {
            LoggingManager.Debug("Trying to unregister packages from components");
            foreach (var component in components)
            {
                LoggingManager.Debug("Trying to unregister packages from component:" + component.Name);
                if (component.Packages != null && component.Packages.Count(p => p.State == State.Removed) > 0)
                {
                    foreach (var removedPackage in component.Packages.Where(p => p.State == State.Removed).ToList())
                    {
                        LoggingManager.Debug("Trying to remove package:" + removedPackage.Id + " from component: " +
                                             component.Name);
                        component.Packages.Remove(removedPackage);
                    }
                }
                if (component.DependentComponents != null)
                    UnRegisterOldPackages(component.DependentComponents);
                LoggingManager.Debug("Unregistered packages from component:" + component.Name);
            }
            LoggingManager.Debug("Unregistered all packages");
        }

        private bool DistributeMessages(IEnumerable<AvailableChannel> channelsFromAPublisher, PublishPackageRequestResponse packageRequestResponse)
        {
            LoggingManager.Debug("Distribute messages from package:" + packageRequestResponse.PackageId);
            RegisterPublisherPackage(channelsFromAPublisher.First(), packageRequestResponse, State.InProgress);
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
                    RegisterSubscriberPackage(channel, packageRequestResponse, State.InProgress);
                    if (channel.SubscriberInfo.Subscriber.ApplyChangePackage(packageRequestResponse).Status)
                    {
                        RegisterSubscriberPackage(channel, packageRequestResponse, State.Distributed);
                    }
                    else
                        result = false;
                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError("Error while applying the changes", ex);
                    result = false;
                }
            }
            LoggingManager.Debug(((result) ? "Message distributed: " : "Message not distributed: ") + packageRequestResponse.PackageId);
            return result;
        }

        private void RegisterSubscriberPackage(AvailableChannel channel, PublishPackageRequestResponse packageRequestResponse, State state)
        {
            var publisherName = string.Format("{0}{1}",
                                              channel.PublisherInfo.
                                                  PublisherInstanceName,
                                              (string.IsNullOrEmpty(
                                                  channel.PublisherInfo.
                                                      EndpointName))
                                                  ? ""
                                                  : "." +
                                                    channel.PublisherInfo.
                                                        EndpointName);
            var subscriberName = string.Format("{0}{1}",
                                               channel.SubscriberInfo.
                                                   SubScriberName,
                                               (string.IsNullOrEmpty(
                                                   channel.SubscriberInfo.
                                                       EndpointName))
                                                   ? ""
                                                   : "." +
                                                     channel.SubscriberInfo.
                                                         EndpointName);

            LoggingManager.Debug("Register package: " + packageRequestResponse.PackageId + " with subscriber: " + subscriberName);

            var existingpublisher = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingpublisher == null)
            {
                LoggingManager.Debug("Publisher doesn't exist:" + publisherName);
                return;
            }
            var existingcomponent = existingpublisher.DependentComponents.FirstOrDefault(c => c.Name == subscriberName);
            if (existingcomponent == null)
            {
                LoggingManager.Debug("Subscriber doesn't exist:" + subscriberName);
                return;
            }

            if (existingcomponent.Packages == null)
                existingcomponent.Packages = new List<Package>();

            var existingPackage = existingcomponent.Packages.FirstOrDefault(p => p.Id == packageRequestResponse.PackageId);
            if (existingPackage != null)
            {
                existingPackage.State = state;
                LoggingManager.Debug("Package exists updating state to: " + state);
                return;
            }
            existingcomponent.Packages.Add(new Package
                                               {
                                                   Id = packageRequestResponse.PackageId,
                                                   State = state,
                                                   PackageMessages =
                                                       GetMessagesForSubscriber(packageRequestResponse.ChangePushItems,
                                                                                existingcomponent.RootPath,
                                                                                packageRequestResponse.SourceRootName)
                                               });
            LoggingManager.Debug("Registered new package: " + packageRequestResponse.PackageId + " on subscriber: " + subscriberName +
                                 " with state: " + state);
        }

        private static List<ChangePushItem> GetMessagesForSubscriber(List<ChangePushItem> changePushItems,
                                                              string targetRootPath, string sourceRootPath)
        {
            var resultPushItems = new List<ChangePushItem>();
            changePushItems.ForEach(c => resultPushItems.Add(new ChangePushItem
                                                                   {
                                                                       AbsolutePath =
                                                                           c.AbsolutePath.Replace(sourceRootPath,
                                                                                                  targetRootPath),
                                                                       OperationType = c.OperationType
                                                                   }));
            return resultPushItems;
        }

        public void InitiateDistributionMap(string mapFile, ComponentResolver componentResolver)
        {
            if (componentResolver == null)
                throw new ArgumentNullException("componentResolver");

            _componentResolver = componentResolver;

            if (string.IsNullOrEmpty(mapFile))
                throw new ArgumentNullException();
            if (!File.Exists(mapFile))
                throw new ArgumentException();
            AvailableChannels = Serializer.DeserializeFromFile<AvailableChannel>(mapFile);
            _allComponents = new List<AvailableComponent>();

            if (AvailableChannels == null || AvailableChannels.Count <= 0)
                throw new Exception("No channels in the map file");
            try
            {
                LoggingManager.Debug("Initiating distributor with map: " + mapFile);
                AvailableChannels.ForEach(CheckChannel);
                LoggingManager.Debug("Initiated Ok.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }

        private void CheckChannel(AvailableChannel availableChannel)
        {
            if (availableChannel.Status == Status.OfflinePermanent)
                return;
            if (!PublisherAlive(availableChannel))
                return;
            if (!SubscriberAlive(availableChannel))
                return;
            if (!DataSourceAlive(availableChannel))
                return;
            availableChannel.Status = Status.Ok;
        }


        private bool DataSourceAlive(AvailableChannel availableChannel)
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
            var subscriberName =
                string.Format("{0}{1}",
                              availableChannel.SubscriberInfo.
                                  SubScriberName,
                              (string.IsNullOrEmpty(
                                  availableChannel.SubscriberInfo.
                                      EndpointName))
                                  ? ""
                                  : "." +
                                    availableChannel.SubscriberInfo.
                                        EndpointName);

            try
            {
                if (CheckDataSourceFromTheSubscriberPointOfView(availableChannel))
                {
                    UpdateDataSource(subscriberName, publisherName, Status.Ok);
                    return true;
                }
                Status dataSourceStatus;
                if (availableChannel.NoOfFailedAttempts < MaxNoOfFailedAttempts)
                {
                    availableChannel.Status = Status.OfflineTemporary;
                    availableChannel.NoOfFailedAttempts++;
                    dataSourceStatus = Status.OfflineTemporary;
                }
                else
                {
                    availableChannel.Status = Status.OfflinePermanent;
                    dataSourceStatus = Status.OfflinePermanent;
                }
                UpdateDataSource(subscriberName, publisherName, dataSourceStatus);
                return false;

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(availableChannel.DataSourceEndpointName, ex);
                availableChannel.Status = Status.OfflinePermanent;
                UpdateDataSource(subscriberName,publisherName,Status.OfflinePermanent);
                return false;
            }
        }

        private static bool CheckDataSourceFromTheSubscriberPointOfView(AvailableChannel availableChannel)
        {
            if (availableChannel.SubscriberInfo.Subscriber == null)
                return false;
            
            return
                availableChannel.SubscriberInfo.Subscriber.TryOpenChannel((string.IsNullOrEmpty(availableChannel.DataSourceEndpointName))?null:new TryOpenChannelRequest
                                                                              {
                                                                                  SourceOfDataEndpointName =
                                                                                      availableChannel.DataSourceEndpointName
                                                                              }).Status;
        }

        private void UpdateDataSource(string subscriberName, string publisherName, Status dataSourceStatus)
        {
            LoggingManager.Debug("Updating datasource for subsriber: " + subscriberName + " to value: " + dataSourceStatus);
            var existingPublisher = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingPublisher == null)
            {
                LoggingManager.Debug("Publisher not found: " + publisherName);
                return;
            }
            if (existingPublisher.DependentComponents == null)
            {
                LoggingManager.Debug("Publisher does not have any subscribers");
                return;
            }
            var existingComponent =
                existingPublisher.DependentComponents.FirstOrDefault(c => c.Name == subscriberName);
            if (existingComponent == null)
            {
                LoggingManager.Debug("Subscriber not found: " + subscriberName);
                return;
            }
            existingComponent.DataSourceStatus = dataSourceStatus;
            LoggingManager.Debug("DataSource updated.");
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
            var availableComponent = new AvailableComponent
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

            ISubscriber subscriber;

            try
            {
                if (string.IsNullOrEmpty(availableChannel.SubscriberInfo.EndpointName))
                {
                    subscriber = (availableChannel.SubscriberInfo.Subscriber) ??
                                 (availableChannel.SubscriberInfo.Subscriber =
                                  _componentResolver.Resolve<ISubscriber>(
                                      availableChannel.SubscriberInfo.SubScriberName));
                }
                else
                {
                    subscriber = (availableChannel.SubscriberInfo.Subscriber) ??
                                 (availableChannel.SubscriberInfo.Subscriber =
                                  _componentResolver.Resolve<ISubscriberProxy>(
                                      availableChannel.SubscriberInfo.SubScriberName,
                                      availableChannel.SubscriberInfo.EndpointName));
                }
                if (!subscriber.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < MaxNoOfFailedAttempts)
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
                availableComponent.RootPath = availableChannel.SubscriberInfo.Subscriber.GetTargetRootFolder().RootFolder;
                UpsertComponent(availableComponent, publisherName);
                return true;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(
                    availableChannel.SubscriberInfo.SubScriberName + availableChannel.SubscriberInfo.EndpointName, ex);
                availableChannel.Status = Status.OfflinePermanent;
                availableComponent.Status = Status.OfflinePermanent;
                return false;
            }
        }

        private void UpsertComponent(AvailableComponent availableComponent, string publisherName)
        {
            LoggingManager.Debug("Upserting component for publisher: " + publisherName);
            var existingPublisher = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingPublisher == null)
            {
                LoggingManager.Debug("Publisher not found: " + publisherName);
                return;
            }
            if (existingPublisher.DependentComponents == null)
                existingPublisher.DependentComponents = new List<AvailableComponent>();
            var existingComponent =
                existingPublisher.DependentComponents.FirstOrDefault(c => c.Name == availableComponent.Name);
            if (existingComponent == null)
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
            var availableComponent = new AvailableComponent
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
                                                            Status = Status.NotChecked,
                                                        };

            IPublisher localPublisher;
            try
            {
                if (string.IsNullOrEmpty(availableChannel.PublisherInfo.EndpointName))
                {
                    availableComponent.IsLocal = true;
                    //the publisher has to be local
                    localPublisher = (availableChannel.PublisherInfo.Publisher) ??
                                     (availableChannel.PublisherInfo.Publisher =
                                      _componentResolver.Resolve<IPublisher>(
                                          availableChannel.PublisherInfo.PublisherInstanceName));
                }
                else
                {
                    availableComponent.IsLocal = false;

                    localPublisher = (availableChannel.PublisherInfo.Publisher) ??
                                     (availableChannel.PublisherInfo.Publisher =
                                      _componentResolver.Resolve<IPublisherProxy>(
                                          availableChannel.PublisherInfo.PublisherInstanceName,
                                          availableChannel.PublisherInfo.EndpointName));
                }
                if (!localPublisher.GetHeartbeat().Status)
                {
                    if (availableChannel.NoOfFailedAttempts < MaxNoOfFailedAttempts)
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
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(
                    availableChannel.PublisherInfo.PublisherInstanceName + availableChannel.PublisherInfo.EndpointName,
                    ex);
                availableChannel.Status = Status.OfflinePermanent;
                availableComponent.Status = Status.OfflinePermanent;
                return false;
            }

        }

        private void UpsertComponent(AvailableComponent availableComponent)
        {
            LoggingManager.Debug("Upserting publisher:" + availableComponent.Name);
            var existingComponent = _allComponents.FirstOrDefault(p => p.Name == availableComponent.Name);
            if (existingComponent == null)
                _allComponents.Add(availableComponent);
            else
            {
                existingComponent.Status = availableComponent.Status;
            }
            LoggingManager.Debug("Publisher upserted.");
        }

        public ListAvailableComponentsTreeResponse ListAvailableComponentsTree()
        {
            LoggingManager.Debug("Listing all available components");
            return new ListAvailableComponentsTreeResponse {Name = Environment.MachineName, AvailablePublishers = _allComponents};
        }

        public void ReEvaluateAllChannels()
        {
            AvailableChannels.Where(c => c.Status != Status.Ok || c.Status != Status.NotChecked).ToList().ForEach(c => c.Status = Status.NotChecked);
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new GetHeartbeatResponse {Status = true};
        }
    }
}