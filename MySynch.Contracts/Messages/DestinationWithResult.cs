using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class DestinationWithResult
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public bool Processed { get; set; }
    }
}
