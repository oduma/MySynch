using System;

namespace MySynch.Common.WCF.Clients
{
    internal class DuplexClientEndpoint<T>
    {
        public DuplexClientEndpoint(Type endpointType, string serverAddress)
        {
            EndpointIdentifier = endpointType.Name + serverAddress;
        }

        public DuplexEndpointChannelFactory<T> Endpoint { get; set; }

        public string EndpointIdentifier { get; set; }

    }
}
