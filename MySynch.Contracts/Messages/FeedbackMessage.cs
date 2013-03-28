using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class FeedbackMessage:ChangePushItem
    {
        [DataMember]
        public bool Processed { get; set; }
    }
}
