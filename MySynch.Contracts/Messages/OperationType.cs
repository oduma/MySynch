using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public enum OperationType
    {
        [EnumMember]
        None=0,
        [EnumMember]
        Insert=1,
        [EnumMember]
        Update=2,
        [EnumMember]
        Delete=3,
        [EnumMember]
        Rename=4
    }
}
