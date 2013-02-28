using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class GetHeartbeatResponse
    {
        [DataMember]
        public bool Status { get; set; }
    }
}
