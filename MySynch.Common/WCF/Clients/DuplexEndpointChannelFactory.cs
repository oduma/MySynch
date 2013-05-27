using System.ServiceModel;

namespace MySynch.Common.WCF.Clients
{
    public class DuplexEndpointChannelFactory<T>
    {
            public EndpointAddress EndpointAddress { get; set; }

            public DuplexChannelFactory<T> ChannelFactory { get; set; }
    }
}
