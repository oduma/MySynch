using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class HeartbeatResponse
    {
        [DataMember]
        public bool Status { get; set; }
    }
}
