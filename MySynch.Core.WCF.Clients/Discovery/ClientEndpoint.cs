using System;
using System.ServiceModel;

namespace MySynch.Core.WCF.Clients.Discovery
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

    public class EndpointChannelFactory
    {
        public EndpointAddress EndpointAddress { get; set; }

        public ChannelFactory ChannelFactory { get; set; }
    }

}
