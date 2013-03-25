using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor.Utils
{
    public class ClientHelper
    {
        public void ConnectToADistributor(EventHandler<ProgressChangedEventArgs> progressChanged, int localDistributorPort, out IDistributorMonitorProxy distributorMonitorProxy, out ListAvailableChannelsResponse availableComponents)
        {
            progressChanged(this, new ProgressChangedEventArgs(0, "Connecting to Distributor at port: " + localDistributorPort));
            distributorMonitorProxy = new DistributorMonitorClient();
            distributorMonitorProxy.InitiateUsingPort(localDistributorPort);
            progressChanged(this, new ProgressChangedEventArgs(0, "Connected to Distributor at port: " + localDistributorPort + " checking registrations..."));
            availableComponents = distributorMonitorProxy.ListAvailableChannels();
            progressChanged(this,
                            new ProgressChangedEventArgs(0,
                                                         string.Format(
                                                             "{0} channels registered at the distributor",
                                                             availableComponents.Channels.Count())));
        }

        public void DisconnectFromADistributor(EventHandler<ProgressChangedEventArgs> progressChanged, int localDistributorPort, IDistributorMonitorProxy distributorMonitorProxy)
        {
            progressChanged(this, new ProgressChangedEventArgs(0, "Disconnecting from Distributor at port: " + localDistributorPort));
            distributorMonitorProxy.DestroyAtPort(localDistributorPort);
            progressChanged(this, new ProgressChangedEventArgs(0, "Disconnected from Distributor at port: " + localDistributorPort + " checking registrations..."));
        }
    }
}
