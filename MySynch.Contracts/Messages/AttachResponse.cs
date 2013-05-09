using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class AttachResponse
    {
        [DataMember]
        public bool RegisteredOk { get; set; }
    }
}
