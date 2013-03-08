using System;
using System.Collections.Generic;
using System.ServiceModel.Discovery;
using MySynch.Common.Logging;

namespace MySynch.Core.WCF.Clients.Discovery
{
    public static class DiscoveryHelper
    {

        public static IEnumerable<EndpointDiscoveryMetadata> FindServices<T>()
        {
            LoggingManager.Debug("Looking for service of type " + typeof(T).FullName);
            try
            {
                DiscoveryClient discoveryClient =
                    new DiscoveryClient(new UdpDiscoveryEndpoint());

                var discoveredServices =
                    discoveryClient.Find(new FindCriteria(typeof(T)));

                discoveryClient.Close();

                var endpointAddress = discoveredServices.Endpoints;
                if (endpointAddress == null || endpointAddress.Count<=0)
                {
                    LoggingManager.Debug("No service of type " + typeof(T).FullName + " found.");
                    return null;
                }
                LoggingManager.Debug("Found services.");
                return endpointAddress;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }


    }
}
