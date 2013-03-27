using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public enum State
    {
        [EnumMember]
        Published=0,
        [EnumMember]
        InProgress,
        [EnumMember]
        Distributed,
        [EnumMember]
        Done
    }
}
