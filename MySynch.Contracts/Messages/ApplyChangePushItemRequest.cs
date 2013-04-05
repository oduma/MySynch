using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ApplyChangePushItemRequest
    {
        [DataMember]
        public ChangePushItem ChangePushItem { get; set; }

        [DataMember]
        public string SourceRootName { get; set; }
    }
}
