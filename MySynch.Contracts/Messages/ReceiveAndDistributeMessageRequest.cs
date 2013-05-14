using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ReceiveAndDistributeMessageRequest
    {
        [DataMember]
        public PublisherMessage PublisherMessage { get; set; }
    }
}
