using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ChangePushItem
    {
        [DataMember]
        public string AbsolutePath { get; set; }

        [DataMember]
        public OperationType OperationType { get; set; }
    }
}
