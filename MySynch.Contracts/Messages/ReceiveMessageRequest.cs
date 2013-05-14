using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ReceiveMessageRequest
    {
        [DataMember]
        public PublisherMessage PublisherMessage { get; set; }
    }
}
