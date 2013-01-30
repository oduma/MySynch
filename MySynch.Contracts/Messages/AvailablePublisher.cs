using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class AvailablePublisher
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsLocal { get; set; }

        [DataMember]
        public Status Status { get; set; }
    }
}
