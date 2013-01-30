using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public enum Status
    {
        [EnumMember]
        NotChecked=0,
        [EnumMember]
        Ok,
        [EnumMember]
        OfflineTemporary,
        [EnumMember]
        OfflinePermanent
    }
}
