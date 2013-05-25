﻿using System;
using System.ComponentModel;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Proxies.Autogenerated.Implementations;
using MySynch.Proxies.Autogenerated.Interfaces;

namespace MySynch.Monitor.Utils
{
    public class ClientHelper
    {
        public void ConnectToABroker(EventHandler<ProgressChangedEventArgs> progressChanged, string brokerName, out IBrokerProxy brokerClient, out ListAllRegistrationsResponse registeredComponents)
        {
            string brokerUrl = string.Format("http://{0}/broker/", brokerName);
            progressChanged(this, new ProgressChangedEventArgs(0, "Connecting to Broker: " + brokerUrl));
            brokerClient = new BrokerClient();
            brokerClient.InitiateUsingServerAddress(brokerUrl);
            progressChanged(this, new ProgressChangedEventArgs(0, "Connected to Broker: " + brokerUrl + " checking registrations..."));
            registeredComponents = brokerClient.ListAllRegistrations();
            var subscribersRegistered =
                string.Join(",\r\n",registeredComponents.Registrations.Where(r => r.ServiceRole == ServiceRole.Subscriber).Select(
                    r => r.ServiceUrl));
            var publishersRegistered =
                string.Join(",\r\n", registeredComponents.Registrations.Where(r => r.ServiceRole == ServiceRole.Publisher).Select(
                    r => r.ServiceUrl));
            progressChanged(this,
                            new ProgressChangedEventArgs(0,
                                                         string.Format(
                                                             "{0} components registered with the broker.\r\nFrom witch:\r\nSubscribers:{1}\r\nPublishers{2}",
                                                             registeredComponents.Registrations.Count,subscribersRegistered,publishersRegistered)));
        }
    }
}
