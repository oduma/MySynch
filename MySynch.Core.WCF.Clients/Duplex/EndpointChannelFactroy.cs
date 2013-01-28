using System.ServiceModel;

namespace MySynch.Core.WCF.Clients.Duplex
{
    public class EndpointChannelFactory<T>
    {
        public EndpointAddress EndpointAddress { get; set; }

        public DuplexChannelFactory<T> ChannelFactory { get; set; }
    }
}
