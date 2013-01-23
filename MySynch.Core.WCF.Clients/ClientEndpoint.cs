using System;

namespace MySynch.Core.WCF.Clients
{
    internal class ClientEndpoint
    {

        public ClientEndpoint(Type endpointType, string endpointName)
        {
            EndpointIdentifier = endpointType.Name + endpointName;
        }

        public EndpointChannelFactory Endpoint { get; set; }

        public string EndpointIdentifier { get; set; }
    }
}
