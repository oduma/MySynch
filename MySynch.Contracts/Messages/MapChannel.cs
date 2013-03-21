using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class MapChannel
    {
        [DataMember]
        public MapChannelComponent PublisherInfo { get; set; }

        [DataMember]
        public MapChannelComponent SubscriberInfo { get; set; }

        [DataMember]
        [XmlIgnore]
        public Status Status { get; set; }
    }
}
