﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Configuration;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies.Autogenerated.Interfaces;
using Sciendo.Common.IOC;
using Sciendo.Common.Logging;

namespace MySynch.Core.Broker
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Broker:IBroker,IBrokerMonitor
    {
        private IEnumerable<Registration> _registrations;
        private MySynchBrokerConfigurationSection _brokerConfiguration;
        private IStore<Registration> _storeHandler;
        internal List<MessageWithDestinations> _receivedMessages;
        private ComponentResolver _componentResolver;
        internal virtual SortedList<string, ISubscriberProxy> InitiatedSubScriberProxies { get; set; }
        private object _lock= new object();
        private IBrokerMonitorCallback _callback;

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true,RootPath=""};
        }

        public void StartMonitoring()
        {
            //Only one callback at a time
            _callback = OperationContext.Current.GetCallbackChannel<IBrokerMonitorCallback>();
        }

        public Broker(MySynchBrokerConfigurationSection storeType, ComponentResolver componentResolver)
        {
            try
            {
                _brokerConfiguration = storeType;
                _componentResolver = componentResolver;
                _storeHandler = _componentResolver.Resolve<IStore<Registration>>(storeType.StoreType);
                _registrations =
                    _storeHandler.GetMethod(AppDomain.CurrentDomain.BaseDirectory +"\\"+_brokerConfiguration.StoreName);
                _receivedMessages=new List<MessageWithDestinations>();
                InitiatedSubScriberProxies= new SortedList<string, ISubscriberProxy>();
                if(OperationContext.Current!=null)
                    _callback = OperationContext.Current.GetCallbackChannel<IBrokerMonitorCallback>();    

            }
            catch (ComponentNotRegieteredException ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                _registrations = new List<Registration>();
            }
        }

        public AttachResponse Attach(AttachRequest request)
        {
            LoggingManager.Debug("Attaching attempt to the Broker...");
            using (LoggingManager.LogSciendoPerformance())
            {
                if (request == null || request.RegistrationRequest == null ||
                    string.IsNullOrEmpty(request.RegistrationRequest.ServiceUrl))
                {
                    LoggingManager.Debug("Not attached due to empty request.");
                    return new AttachResponse {RegisteredOk = false};
                }
                try
                {
                    if (_registrations.Any(r => r.ServiceUrl == request.RegistrationRequest.ServiceUrl))
                    {
                        LoggingManager.Debug("Attached to old " + request.RegistrationRequest.ServiceUrl);
                        return new AttachResponse {RegisteredOk = true};
                    }
                    _registrations =
                        _registrations.ToList().AddRegistration(request.RegistrationRequest).SaveAndReturn(
                            AppDomain.CurrentDomain.BaseDirectory + "\\" + _brokerConfiguration.StoreName, _storeHandler.StoreMethod);
                    if(_callback!=null)
                        _callback.NotifyNewRegistration(request.RegistrationRequest,_registrations.ToList());
                    LoggingManager.Debug("Attached to new " + request.RegistrationRequest.ServiceUrl);
                    return new AttachResponse {RegisteredOk = true};
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                    LoggingManager.Debug("Not attached " + request.RegistrationRequest.ServiceUrl);
                    return new AttachResponse {RegisteredOk = false};
                }
                finally
                {
                    LoggingManager.Debug("Attached services: " + string.Join(", ",_registrations.Select(r=>r.ServiceUrl)));
                }
            }
        }

        public DetachResponse Detach(DetachRequest request)
        {
            LoggingManager.Debug("Detaching attempt to the Broker...");
            if (request == null || string.IsNullOrEmpty(request.ServiceUrl))
            {
                LoggingManager.Debug("Not detached ");
                return new DetachResponse { Status = false };
            }
            try
            {
                if (!_registrations.Any(r => r.ServiceUrl == request.ServiceUrl))
                {
                    LoggingManager.Debug("Not detached " + request.ServiceUrl);
                    return new DetachResponse { Status = false };
                }
                Registration deletedCopy;
                _registrations =
                    _registrations.ToList().RemoveRegistration(request.ServiceUrl,out deletedCopy).SaveAndReturn(
                        _brokerConfiguration.StoreName,_storeHandler.StoreMethod);
                LoggingManager.Debug("Detached :" + request.ServiceUrl);
                if (_callback != null && deletedCopy!=null)
                    _callback.NotifyRemoveRegistration(deletedCopy,_registrations.ToList());
                return new DetachResponse { Status = true };
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                LoggingManager.Debug("Not detached " + request.ServiceUrl);
                return new DetachResponse { Status = false };
            }
            finally
            {
                LoggingManager.Debug("Attached services: " + string.Join(", ", _registrations.Select(r => r.ServiceUrl)));
            }
        }

        public void ReceiveAndDistributeMessage(ReceiveAndDistributeMessageRequest request)
        {
            LoggingManager.Debug("Received request from publisher.");

            MessageWithDestinations messageToBeProcessed =
                _receivedMessages.FirstOrDefault(m => m.MessageId == request.PublisherMessage.MessageId);
            if (messageToBeProcessed==null)
            {
                messageToBeProcessed = new MessageWithDestinations
                                                     {
                                                         AbsolutePath = request.PublisherMessage.AbsolutePath,
                                                         MessageId = request.PublisherMessage.MessageId,
                                                         OperationType = request.PublisherMessage.OperationType,
                                                         SourceOfMessageUrl =
                                                             request.PublisherMessage.SourceOfMessageUrl,
                                                         SourcePathRootName =
                                                             request.PublisherMessage.SourcePathRootName,
                                                         Destinations =
                                                             new List<DestinationWithResult>()
                                                     };
                _receivedMessages.Add(messageToBeProcessed);
            }

            DistributeMessageToAllAvailableSubscribers(messageToBeProcessed);
            LoggingManager.Debug("Request forwarded to all subscribers.");
        }

        public void MessageReceivedFeedback(MessageReceivedFeedbackRequest request)
        {
            if(request==null|| string.IsNullOrEmpty(request.DestinationUrl) || request.PackageId==Guid.Empty)
                throw new ArgumentNullException("request");
            lock (_lock)
            {
                var msg = _receivedMessages.FirstOrDefault(m => m.MessageId == request.PackageId);
                if (msg != null)
                {
                    if (msg.Destinations == null || msg.Destinations.Count() == 0)
                        return;
                    if (msg.Destinations.Any(d => d.Url != request.DestinationUrl))
                    {
                        var dst = msg.Destinations.FirstOrDefault(d => d.Url == request.DestinationUrl);
                        if (dst != null)
                        {
                            msg.Destinations.Remove(dst);
                            if (_callback != null)
                                _callback.NotifyMessageUpdate(msg,_receivedMessages);
                        }
                    }
                    else
                    {
                        var deletedMessage = new MessageWithDestinations();
                        deletedMessage = msg;
                        _receivedMessages.Remove(msg);
                        if (_callback != null)
                            _callback.NotifyMessageDelete(deletedMessage,_receivedMessages);
                    }
                }
            }
        }

        internal void DistributeMessageToAllAvailableSubscribers(MessageWithDestinations publisherMessage)
        {
            var availableSubscribers =
                _registrations.Where(
                    r =>
                    r.ServiceRole == ServiceRole.Subscriber && r.OperationTypes.Contains(publisherMessage.OperationType));
            if (!availableSubscribers.Any())
                return;

            publisherMessage.Destinations =
                availableSubscribers.Select(s => new DestinationWithResult {Processed = false, Url = s.ServiceUrl}).
                    ToList();
            var destinations =
                availableSubscribers.Select(
                    s => new AddresedMessage {DestinationUrl = s.ServiceUrl, ProcessedByDestination = false,OriginalMessage=publisherMessage}).ToList();
            List<Task> tasks= new List<Task>();
            LoggingManager.Debug("Trying to distribute to: " + string.Join(", ",destinations.Select(d=>d.DestinationUrl)));
            foreach (var destination in destinations)
            {
                AddresedMessage destination1 = destination;
                Task task = new Task(()=>DistributeMessageToSubscriber(destination1));
                tasks.Add(task);
                task.Start();
            }
        }

        internal void DistributeMessageToSubscriber(object destination)
        {
            AddresedMessage subscriberAddresedMessage = ((AddresedMessage) destination);
            var subscriberRemote = GetOrCreateAProxy(subscriberAddresedMessage.DestinationUrl);
            if (subscriberRemote == null)
            {
                LoggingManager.Debug("Did not distribute to subscriber: " + subscriberAddresedMessage.DestinationUrl +
                                     ". Subscriber dead or unreachable at the moment.");
                subscriberAddresedMessage.ProcessedByDestination = false;
                MarkAsProcessedByDestination(subscriberAddresedMessage);
                return;
            }
            try
            {
                ReceiveMessageRequest request = new ReceiveMessageRequest { PublisherMessage = subscriberAddresedMessage.OriginalMessage };
                var response = subscriberRemote.ReceiveMessage(request);
                subscriberAddresedMessage.ProcessedByDestination = response.Success;
                MarkAsProcessedByDestination(subscriberAddresedMessage);
                LoggingManager.Debug("Distributed to subscriber: " + subscriberAddresedMessage.DestinationUrl);

            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                subscriberAddresedMessage.ProcessedByDestination = false;
                MarkAsProcessedByDestination(subscriberAddresedMessage);
            }
        }

        private void MarkAsProcessedByDestination(AddresedMessage subscriberAddresedMessage)
        {
            lock (_lock)
            {
                var msg =
                    _receivedMessages.First(m => m.MessageId == subscriberAddresedMessage.OriginalMessage.MessageId);
                if (msg == null)
                    return;
                var dest = msg.Destinations.FirstOrDefault(d => d.Url == subscriberAddresedMessage.DestinationUrl);
                if (dest==null)
                    msg.Destinations.Add(new DestinationWithResult
                                             {
                                                 Url = subscriberAddresedMessage.DestinationUrl,
                                                 Processed = subscriberAddresedMessage.ProcessedByDestination
                                             });
                else
                    dest.Processed = subscriberAddresedMessage.ProcessedByDestination;
                if (_callback != null)
                    _callback.NotifyNewMessage(msg,_receivedMessages);
                if (!subscriberAddresedMessage.ProcessedByDestination 
                    && _registrations.Any(r=>r.ServiceUrl==subscriberAddresedMessage.DestinationUrl))
                    Detach(new DetachRequest {ServiceUrl = subscriberAddresedMessage.DestinationUrl});
            }
        }

        internal ISubscriberProxy GetOrCreateAProxy(string destinationUrl)
        {
            lock (_lock)
            {
                try
                {
                    if (InitiatedSubScriberProxies.ContainsKey(destinationUrl))
                    {
                        InitiatedSubScriberProxies[destinationUrl].Reset();
                        return InitiatedSubScriberProxies[destinationUrl];
                    }

                    var subscriberRemote = _componentResolver.Resolve<ISubscriberProxy>("ISubscriber.Remote");
                    subscriberRemote.InitiateUsingServerAddress(destinationUrl);
                    InitiatedSubScriberProxies.Add(destinationUrl, subscriberRemote);
                    return subscriberRemote;
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                    return null;
                }
            }
        }

        public ListAllRegistrationsResponse ListAllRegistrations()
        {
            return new ListAllRegistrationsResponse {Registrations = _registrations.ToList()};
        }

        public ListAllMessagesResponse ListAllMessages()
        {
            return new ListAllMessagesResponse {AvailableMessages = _receivedMessages};
        }
    }
}
