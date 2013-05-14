using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    [KnownType(typeof(PublisherMessage))]
    public class BrokerMessage:PublisherMessage
    {
        [DataMember]
        public List<Destination> Destinations { get; set; }
    }
}
