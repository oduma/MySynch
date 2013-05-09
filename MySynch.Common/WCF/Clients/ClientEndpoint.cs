using System;

namespace MySynch.Common.WCF.Clients
{
    internal class ClientEndpoint
    {

        public ClientEndpoint(Type endpointType, string serverAddress)
        {
            EndpointIdentifier = endpointType.Name + serverAddress;
        }

        public EndpointChannelFactory Endpoint { get; set; }

        public string EndpointIdentifier { get; set; }
    }
}
