using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Proxies.Interfaces;

namespace MySynch.Core.Distributor
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Distributor : IDistributorMonitor
    {
        private MySynchComponentResolver _componentResolver = new MySynchComponentResolver();
        internal bool StillProcessing;
        internal bool AllProcessedOk;
        private const int MaxNoOfFailedAttempts = 3;
        public List<AvailableChannel> AvailableChannels { get; private set; }

        public void DistributeMessages()
        {
            try
            {
                LoggingManager.Debug("Starting to distribute messages ...");
                //For test only keep old trace
                AvailableChannels.ForEach((c)=>c.PublisherInfo.CurrentPackage = c.SubscriberInfo.CurrentPackage = null);
                //Recheck the AvailableChannels
                AvailableChannels.ForEach(CheckChannel);

                var availableChannels = AvailableChannels.Where(c => c.Status == Status.Ok);
                if (availableChannels.Count() == 0)
                    return;
                //the channels have to be processed in the order of the publisher
                var publisherGroups =
                    availableChannels.GroupBy(
                        c => c.PublisherInfo.InstanceName + "-" + c.PublisherInfo.Port);
                foreach (var publisherGroup in publisherGroups)
                {

                    IGrouping<string, AvailableChannel> @group = publisherGroup;
                    var publisherChannelsNotAvailable =
                        AvailableChannels.Count(
                            c =>
                            ((c.PublisherInfo.InstanceName + "-" + c.PublisherInfo.Port) ==
                             group.Key) && c.Status != Status.Ok);

                    var currentPublisher = publisherGroup.First().Publisher;
                    var currentPublisherInfo = publisherGroup.First().PublisherInfo;

                    var packagePublished = currentPublisher.PublishPackage();

                    if (packagePublished == null)
                        continue;
                    RegisterPackageForComponent(currentPublisherInfo, packagePublished, State.Published);

                    if (DistributeMessages(publisherGroup.Select(g => g), packagePublished) &&
                        publisherChannelsNotAvailable == 0)
                        //Publisher's messages not needed anymore
                    {
                        RegisterPackageForComponent(currentPublisherInfo, packagePublished,
                                                 State.Distributed);
                        currentPublisher.RemovePackage(packagePublished);
                        RegisterPackageForComponent(currentPublisherInfo, packagePublished,
                                                 State.Done);
                        foreach (var channel in publisherGroup)
                            RegisterPackageForComponent(channel.SubscriberInfo, packagePublished, State.Done);
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

        private static void RegisterPackageForComponent(MapChannelComponent component, PublishPackageRequestResponse packageRequestResponse, State state)
        {
            string componentName = GetInstanceName(component);
            LoggingManager.Debug("Register package: " + packageRequestResponse.PackageId + " with component: " + componentName);

            if (component.CurrentPackage == null)
                component.CurrentPackage = new Package {Id = packageRequestResponse.PackageId,State=state};
            else
                component.CurrentPackage.State = state;

            component.CurrentPackage.PackageMessages = (component.InstanceName.Contains("IPublisher"))
                                                           ? packageRequestResponse.ChangePushItems.Select(
                                                               p => new FeedbackMessage
                                                                        {
                                                                            AbsolutePath = p.AbsolutePath,
                                                                            OperationType = p.OperationType,
                                                                            Processed = false
                                                                        }).ToList()
                                                           : new List<FeedbackMessage>();
            LoggingManager.Debug("Registered new package: " + packageRequestResponse.PackageId + " on component: " + componentName +
                                 " with state: " + state);
        }

        private static string GetInstanceName(MapChannelComponent baseInfo)
        {
            return string.Format("{0}{1}",
                                 baseInfo.
                                     InstanceName,
                                 (baseInfo.
                                      Port==0)
                                     ? ""
                                     : "." +
                                       baseInfo.
                                           Port);
        }


        private bool DistributeMessages(IEnumerable<AvailableChannel> channelsFromAPublisher, PublishPackageRequestResponse packageRequestResponse)
        {
            LoggingManager.Debug("Distribute messages from package:" + packageRequestResponse.PackageId);
            RegisterPackageForComponent(channelsFromAPublisher.First().PublisherInfo, packageRequestResponse, State.InProgress);
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
                    RegisterPackageForComponent(channel.SubscriberInfo, packageRequestResponse, State.InProgress);
                    AllProcessedOk = true;
                    foreach (var changePushItem in packageRequestResponse.ChangePushItems)
                    {
                        var response = channel.Subscriber.ApplyChangePushItem(new ApplyChangePushItemRequest
                        {
                            ChangePushItem = changePushItem,
                            SourceRootName =
                                packageRequestResponse.SourceRootName
                        });
                        AllProcessedOk = (AllProcessedOk) ? response.Success : false;
                        RegisterMessageInPackage(response,packageRequestResponse.PackageId);
                    }


                    RegisterPackageForComponent(channel.SubscriberInfo, packageRequestResponse, State.Distributed);
                    result = AllProcessedOk;
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
        
        public void InitiateDistributionMap(string mapFile, MySynchComponentResolver componentResolver)
        {
            if (componentResolver == null)
                throw new ArgumentNullException("componentResolver");

            _componentResolver = componentResolver;

            if (string.IsNullOrEmpty(mapFile))
                throw new ArgumentNullException();
            if (!File.Exists(mapFile))
                throw new ArgumentException();
            AvailableChannels = Serializer.DeserializeFromFile<AvailableChannel>(mapFile);

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
            availableChannel.Publisher = InitiateComponent<IPublisher, IPublisherProxy>(availableChannel.PublisherInfo,
                                                                                     availableChannel.Publisher);
            if (availableChannel.Publisher==null)
            {
                SetChannelOfflineStatus(availableChannel);
                return;
            }
            availableChannel.Subscriber = InitiateComponent<ISubscriber, ISubscriberProxy>(
                availableChannel.SubscriberInfo, availableChannel.Subscriber,true);
            if (availableChannel.Subscriber==null)
            {
                SetChannelOfflineStatus(availableChannel);
                return;
            }
            if (!CheckDataSourceFromTheSubscriberPointOfView(availableChannel))
            {
                SetChannelOfflineStatus(availableChannel);
                return;
            }
            availableChannel.Status = Status.Ok;
        }

        private static void SetChannelOfflineStatus(AvailableChannel channel)
        {
            if (channel.NoOfFailedAttempts < MaxNoOfFailedAttempts)
            {
                channel.Status = Status.OfflineTemporary;
                channel.NoOfFailedAttempts++;
            }
            else
            {
                channel.Status = Status.OfflinePermanent;
            }
        }

        private static bool CheckDataSourceFromTheSubscriberPointOfView(AvailableChannel availableChannel)
        {
            try
            {
                if (availableChannel.Subscriber == null)
                    return false;

                return
                    availableChannel.Subscriber.TryOpenChannel((availableChannel.PublisherInfo.Port == 0) ? null : new TryOpenChannelRequest
                    {
                        SourceOfDataPort =
                            availableChannel.PublisherInfo.Port
                    }).Status;

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(
                    "Communication not available between: " + GetInstanceName(availableChannel.PublisherInfo) + " and: " +
                    GetInstanceName(availableChannel.SubscriberInfo), ex);
                return false;
            }
        }


        private T InitiateComponent<T,TProxy>(MapChannelComponent component,T componentInstance, bool useDuplexVersion=false) where T:class,ICommunicationComponent where TProxy:T 
        {
            try
            {
                if (component.Port==0)
                {
                    if(componentInstance==null)
                        componentInstance =
                                  _componentResolver.Resolve<T>(
                                      component.InstanceName);
                }
                else
                {
                    if (componentInstance == null)
                    {
                        if(!useDuplexVersion)
                        {
                            componentInstance =
                                  _componentResolver.Resolve<TProxy>(
                                      component.InstanceName,
                                      component.Port);
                        }
                        else
                        {
                            componentInstance =
                                  _componentResolver.Resolve<TProxy>(
                                      component.InstanceName,
                                      component.Port);
                        }
                    }
                }
                var heartbeatResponse = componentInstance.GetHeartbeat();
                if (!heartbeatResponse.Status)
                {
                    component.Status = Status.OfflineTemporary;
                    return null;
                }
                component.RootPath = heartbeatResponse.RootPath;
                component.Status = Status.Ok;
                return componentInstance;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(
                    component.InstanceName + component.Port, ex);
                component.Status = Status.OfflinePermanent;
                return null;
            }
        }

        public ListAvailableChannelsResponse ListAvailableChannels()
        {
            LoggingManager.Debug("Listing all available channels");
            List<MapChannel> mapChannels = AvailableChannels.Select(ConvertToMapChannel).ToList();
            return new ListAvailableChannelsResponse {Name = Environment.MachineName, Channels = mapChannels};
        }


        private void RegisterMessageInPackage(ApplyChangePushItemResponse message, Guid packageId)
        {
            var subscriberPackage = AvailableChannels.FirstOrDefault(c => c.SubscriberInfo.CurrentPackage.Id == packageId);
            if (subscriberPackage != null)
            {
                var existingMessage =
                    subscriberPackage.SubscriberInfo.CurrentPackage.PackageMessages.FirstOrDefault(m => m.AbsolutePath == message.ChangePushItem.AbsolutePath);
                if (existingMessage != null)
                    existingMessage.Processed = message.Success;
                else
                {
                    subscriberPackage.SubscriberInfo.CurrentPackage.PackageMessages.Add(new FeedbackMessage
                                                              {
                                                                  AbsolutePath = message.ChangePushItem.AbsolutePath,
                                                                  OperationType = message.ChangePushItem.OperationType,
                                                                  Processed = message.Success
                                                              });
                }
            }
        }

        private static MapChannel ConvertToMapChannel(AvailableChannel availableChannel)
        {
            return new MapChannel
                       {
                           PublisherInfo = availableChannel.PublisherInfo,
                           Status = availableChannel.Status,
                           SubscriberInfo = availableChannel.SubscriberInfo
                       };
        }

        public IEnumerable<MapChannel> GetCurrentMap()
        {
            LoggingManager.Debug("Trying to get the map");
            try
            {
                return AvailableChannels.Select(c => (MapChannel)c);

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);   
                return null;
            }
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new GetHeartbeatResponse {Status = true};
        }
    }
}