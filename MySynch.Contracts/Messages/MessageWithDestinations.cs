using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    [KnownType(typeof(PublisherMessage))]
    public class MessageWithDestinations:PublisherMessage
    {
        [DataMember]
        public List<DestinationWithResult> Destinations { get; set; }
    }
}
