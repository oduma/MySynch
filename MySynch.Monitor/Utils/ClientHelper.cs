﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.Models;
using MySynch.Proxies.Autogenerated.Implementations;
using MySynch.Proxies.Autogenerated.Interfaces;
using System.Linq;

namespace MySynch.Monitor.Utils
{
    public static class ClientHelper
    {
        internal static IBrokerMonitorProxy ConnectToADuplexBroker(EventHandler<ProgressChangedEventArgs> progressChanged, string brokerMonitorUrl, InstanceContext callbackInstance)
        {
            if (string.IsNullOrEmpty(brokerMonitorUrl))
                return null;
            var message = "Connecting to Broker monitor: " + brokerMonitorUrl;
            progressChanged(callbackInstance, new ProgressChangedEventArgs(0, new NotificationModel{DateOfEvent = DateTime.Now, Message=message, Source=ComponentType.Broker}));
            IBrokerMonitorProxy brokerClient = new BrokerMonitorClient();
            brokerClient.InitiateDuplexUsingServerAddress(brokerMonitorUrl,callbackInstance);
            try
            {
                if (brokerClient.GetHeartbeat().Status)
                {
                    message = "Connected to Broker monitor: " + brokerMonitorUrl;
                    progressChanged(callbackInstance, new ProgressChangedEventArgs(0, new NotificationModel { DateOfEvent = DateTime.Now, Message = message, Source = ComponentType.Broker }));
                    return brokerClient;
                }
                message = "Not connected to Broker monitor: " + brokerMonitorUrl;
                progressChanged(callbackInstance, new ProgressChangedEventArgs(0, new NotificationModel { DateOfEvent = DateTime.Now, Message = message, Source = ComponentType.Broker }));
                return null;
            }
            catch (Exception ex)
            {
                message = "Not connected to Broker monitor: " + brokerMonitorUrl;
                progressChanged(callbackInstance, new ProgressChangedEventArgs(0, new NotificationModel { DateOfEvent = DateTime.Now, Message = message, Source = ComponentType.Broker }));
                return null;
            }
        }

        public static ComponentType ConvertToComponentType(ServiceRole serviceRole)
        {
            return (serviceRole == ServiceRole.Subscriber) ? ComponentType.Subscriber : ComponentType.Publisher;
        }

        internal static IEnumerable<RegistrationModel> ConvertToRegistrations(IEnumerable<Registration> registrations)
        {
            if (registrations == null)
                return new List<RegistrationModel>();
            return registrations.Select(r=>new RegistrationModel
                {
                    Operations =
                        string.Join(",",
                                    r.OperationTypes.Select(o => o.ToString())),
                    ServiceRole = r.ServiceRole,
                    ServiceUrl = r.ServiceUrl
                });
        }

        internal static IEnumerable<MessageModel> ConvertToMessageModels(IEnumerable<MessageWithDestinations> messages)
        {
            if (messages == null)
                return new List<MessageModel>();
            return messages.SelectMany(m => m.Destinations,
                                (m, d) =>
                                new MessageModel
                                    {
                                        File = m.AbsolutePath,
                                        FromUrl = m.SourceOfMessageUrl,
                                        MessageId = m.MessageId,
                                        OperationType = m.OperationType,
                                        Processed = d.Processed,
                                        ToUrl = d.Url
                                    });
        }

        private static IComponentMonitorProxy ConnectToDuplexComponent(EventHandler<ProgressChangedEventArgs> progressChanged, string componentUrl, InstanceContext callbackInstance, ServiceRole componentRole)
        {
            if (string.IsNullOrEmpty(componentUrl))
                return null;
            var message = "Connecting to " + componentRole + " monitor: " + componentUrl;
            progressChanged(callbackInstance, new ProgressChangedEventArgs(0, new NotificationModel { DateOfEvent = DateTime.Now, Message = message, Source = ConvertToComponentType(componentRole) }));
            IComponentMonitorProxy componentMonitorClient = new ComponentMonitorClient();
            componentMonitorClient.InitiateDuplexUsingServerAddress(componentUrl, callbackInstance);
            try
            {
                if(componentMonitorClient.GetHeartbeat().Status)
                    return componentMonitorClient;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static IComponentMonitorProxy ConnectToADuplexComponentAndStartMonitoring(EventHandler<ProgressChangedEventArgs> progressChanged, string componentUrl, InstanceContext callbackInstance, ServiceRole componentRole)
        {
            var componentMonitorProxy = ClientHelper.ConnectToDuplexComponent(progressChanged, componentUrl,
                                                                 callbackInstance, componentRole);
            var message = string.Empty;
            try
            {
                if (componentMonitorProxy != null)
                {
                    componentMonitorProxy.StartMonitoring();
                    message = "Connected to " + componentRole + " monitor: " + componentUrl;
                    progressChanged(callbackInstance,
                                    new ProgressChangedEventArgs(0,
                                                                 new NotificationModel
                                                                 {
                                                                     DateOfEvent = DateTime.Now,
                                                                     Message = message,
                                                                     Source = ClientHelper.ConvertToComponentType(componentRole)
                                                                 }));
                    return componentMonitorProxy;
                }
                message = "Not Connected to " + componentRole + " monitor: " + componentUrl;
                progressChanged(callbackInstance,
                                new ProgressChangedEventArgs(0,
                                                             new NotificationModel
                                                             {
                                                                 DateOfEvent = DateTime.Now,
                                                                 Message = message,
                                                                 Source = ClientHelper.ConvertToComponentType(componentRole)
                                                             }));
                return null;
            }
            catch
            {
                message = "Not Connected to " + componentRole + " monitor: " + componentUrl;
                progressChanged(callbackInstance,
                                new ProgressChangedEventArgs(0,
                                                             new NotificationModel
                                                             {
                                                                 DateOfEvent = DateTime.Now,
                                                                 Message = message,
                                                                 Source = ClientHelper.ConvertToComponentType(componentRole)
                                                             }));
                return null;
            }

        }


    }
}
