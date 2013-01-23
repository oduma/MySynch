using System.ServiceModel;

namespace MySynch.Core.WCF.Clients
{
    public class EndpointChannelFactory
    {
        public EndpointAddress EndpointAddress { get; set; }

        public ChannelFactory ChannelFactory { get; set; }
    }
}
