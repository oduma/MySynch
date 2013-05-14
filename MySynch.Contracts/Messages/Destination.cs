using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class Destination
    {
        [DataMember]
        public string DestinationUrl { get; set; }

        [DataMember]
        public bool ProcessedByDestination { get; set; }

        [DataMember]
        public PublisherMessage ParentMessage { get; set; }
    }
}
