using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class DetachResponse
    {
        [DataMember]
        public bool Status { get; set; }
    }
}
