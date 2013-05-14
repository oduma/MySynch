using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ReceiveMessageResponse
    {
        [DataMember]
        public bool Success { get; set; }
    }
}
