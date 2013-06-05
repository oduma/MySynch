﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows.Threading;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Proxies.Autogenerated.Implementations;
using MySynch.Proxies.Autogenerated.Interfaces;
using System.Linq;

namespace MySynch.Monitor.Utils
{
    public class ClientHelper
    {
        internal IBrokerMonitorProxy ConnectToADuplexBroker(EventHandler<ProgressChangedEventArgs> progressChanged, string brokerMonitorUrl, InstanceContext callbackInstance)
        {
            var message = "Connecting to Broker monitor: " + brokerMonitorUrl;
            progressChanged(this, new ProgressChangedEventArgs(0, new GenericMessageModel{Message=message, Source=ComponentType.Broker}));
            IBrokerMonitorProxy brokerClient = new BrokerMonitorClient();
            brokerClient.InitiateDuplexUsingServerAddress(brokerMonitorUrl,callbackInstance);
            message = "Connected to Broker monitor: " + brokerMonitorUrl;
            progressChanged(this, new ProgressChangedEventArgs(0, new GenericMessageModel{Message=message,Source=ComponentType.Broker}));
            return brokerClient;
        }

        public static ComponentType ConvertToComponentType(ServiceRole serviceRole)
        {
            return (serviceRole == ServiceRole.Subscriber) ? ComponentType.Subscriber : ComponentType.Publisher;
        }

        internal static IEnumerable<RegistrationModel> ConvertToObservableCollection(List<Registration> registrations)
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

        internal static ObservableCollection<MessageModel> ConvertToObservableCollection(List<MessageWithDestinations> messages)
        {
            ObservableCollection<MessageModel> messageModels = new ObservableCollection<MessageModel>();
            if (messages == null)
                return messageModels;
            foreach (MessageWithDestinations message in messages)
            {
                foreach (DestinationWithResult destination in message.Destinations)
                {
                    messageModels.Add(new MessageModel
                    {
                        File = message.AbsolutePath,
                        FromUrl = message.SourceOfMessageUrl,
                        MessageId = message.MessageId,
                        OperationType = message.OperationType,
                        ToUrl = destination.Url,
                        Processed = destination.Processed
                    });

                }
            }
            return messageModels;
        }

    }
}
