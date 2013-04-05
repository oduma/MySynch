using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ApplyChangePushItemResponse
    {
        [DataMember]
        public ChangePushItem ChangePushItem { get; set; }

        [DataMember]
        public bool Success { get; set; }
    }
}
