using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class DetachRequest
    {
        [DataMember]
        public string ServiceUrl { get; set; }
    }
}
