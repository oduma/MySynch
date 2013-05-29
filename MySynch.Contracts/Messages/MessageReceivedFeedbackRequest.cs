using System;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class MessageReceivedFeedbackRequest
    {
        [DataMember]
        public Guid PackageId { get; set; }

        [DataMember]
        public string DestinationUrl { get; set; }
    }
}
