using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Castle.Core.Internal;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Core.Utilities;
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
            //Recheck the AvailableChannels
            AvailableChannels.ForEach(CheckChannel);

            var availableChannels = AvailableChannels.Where(c => c.Status == Status.Ok);
            if(availableChannels==null || availableChannels.Count()==0)
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

                if(packagePublished==null)
                    continue;
                IDistributorCallbacks callbacks = OperationContext.Current.GetCallbackChannel<IDistributorCallbacks>();

                callbacks.PackagePublished(packagePublished);

                if (DistributeMessages(publisherGroup.Select(g => g), packagePublished) && publisherChannelsNotAvailable==0)
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
                try
                {
                    if (channel.SubscriberInfo.Subscriber.ApplyChangePackage(package, channel.SubscriberInfo.TargetRootFolder,
                                                                         channel.CopyStrategy.Copy))
                    {
                        result = true;
                        IDistributorCallbacks callbacks = OperationContext.Current.GetCallbackChannel<IDistributorCallbacks>();

                        callbacks.PackageApplyed(package);

                    }
                    else
                        result = false;
                }
                catch (Exception)
                {
                    result = false;                    
                }
            }
            return result;
        }

        public void InitiateDistributionMap(string mapFile,ComponentResolver componentResolver)
        {
            if(componentResolver==null)
                throw new ArgumentNullException("componentResolver");
            CheckRegistration(componentResolver);
            _componentResolver = componentResolver;
           
            if(string.IsNullOrEmpty(mapFile))
                throw new ArgumentNullException();
            if(!File.Exists(mapFile))
                throw new ArgumentException();
            AvailableChannels = Serializer.DeserializeFromFile<AvailableChannel>(mapFile);
            _allComponents=new List<AvailableComponent>();

            if(AvailableChannels==null || AvailableChannels.Count<=0)
                throw new Exception("No channels in the map file");
            AvailableChannels.ForEach(CheckChannel);
            //Initialize all the active channels
            AvailableChannels.Where(c=>c.Status==Status.Ok).ForEach(InitializeChannel);
        }

        private static void CheckRegistration(ComponentResolver componentResolver)
        {
            componentResolver.ResolveAll<IPublisher>();
            componentResolver.ResolveAll<IPublisherProxy>();
            componentResolver.ResolveAll<ICopyStrategy>();
            componentResolver.ResolveAll<IChangeApplyer>();
            componentResolver.ResolveAll<ISubscriberProxy>();

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
                //the publisher has to be local
                var subscriberLocal = _componentResolver.Resolve<IChangeApplyer>(availableChannel.SubscriberInfo.SubScriberName);
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
            var existingPublisher = _allComponents.FirstOrDefault(c => c.Name == publisherName);
            if (existingPublisher == null)
                return;
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
                //the publisher has to be local
                var localPublisher = _componentResolver.Resolve<IPublisher>(availableChannel.PublisherInfo.PublisherInstanceName);
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

            var remotePublisher = _componentResolver.Resolve<IPublisherProxy>(availableChannel.PublisherInfo.PublisherInstanceName, 
                                                                                availableChannel.PublisherInfo.EndpointName);
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
            var existingComponent = _allComponents.FirstOrDefault(p => p.Name == availableComponent.Name);
            if(existingComponent==null)
                _allComponents.Add(availableComponent);
            else
            {
                existingComponent.Status = availableComponent.Status;
            }
        }

        public DistributorComponent ListAvailableComponentsTree()
        {
            return new DistributorComponent {Name = Environment.MachineName, AvailablePublishers = _allComponents};
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }
    }
}
